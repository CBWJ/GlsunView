using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.CommService;
using GlsunView.Models;
using System.Web.Script.Serialization;
using System.ComponentModel;

namespace GlsunView.Controllers
{
    public class EDFACardController : Controller
    {
        // GET: EDFACard
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip">设备地址</param>
        /// <param name="port">端口</param>
        /// <param name="slot">槽位</param>
        /// <returns></returns>
        public ActionResult Index(int did, int slot)
        {
            Device d = null;
            using(var ctx = new GlsunViewEntities())
            {
                d = ctx.Device.Find(did);
            }
            EDFAInfo edfaInfo = new EDFAInfo();
            TcpClientService tcp = new TcpClientService(d.DAddress, d.DPort.Value);
            EDFACommService service = new EDFACommService(tcp, slot);
            try
            {
                tcp.Connect();
                edfaInfo.RefreshData(service);
            }
            catch(Exception ex)
            {

            }
            ViewBag.Did = d.ID;
            ViewBag.EndPoint = d.DAddress + ":" + d.DPort.Value + ":" + slot.ToString();
            return View(edfaInfo);
        }
        [HttpPost]
        public ActionResult SetParam(string endpoint, string name, string value, int did)
        {
            JsonResult result = new JsonResult();
            bool bSuccess = false;
            string operation = "";
            int slot = 0;
            try
            {
                var arrPoint = endpoint.Split(':');
                EDFAInfo edfaInfo = new EDFAInfo();
                if (arrPoint.Length == 3)
                {
                    slot = int.Parse(arrPoint[2]);
                    TcpClientService tcp = new TcpClientService(arrPoint[0], int.Parse(arrPoint[1]), 200);
                    EDFACommService service = new EDFACommService(tcp, slot);
                    tcp.Connect();
                    edfaInfo.RefreshData(service);
                    string methodName = "Set" + name.Replace("_", "");
                    var methodInfo = service.GetType().GetMethod(methodName);
                    if (methodInfo != null)
                    {
                        //获取设置项
                        object[] arrDescription = methodInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if(arrDescription != null && arrDescription.Length > 0)
                        {
                            DescriptionAttribute desc = (DescriptionAttribute)arrDescription[0];
                            if (desc != null)
                            {
                                operation = desc.Description;
                            }
                        }
                        //获取方法参数信息
                        var paramInfo = methodInfo.GetParameters();
                        object[] paramObject = new object[paramInfo.Length];
                        int i = 0;
                        foreach (var e in paramInfo)
                        {
                            paramObject[i] = Convert.ChangeType(value, e.ParameterType);
                        }

                        var ret = (bool)methodInfo.Invoke(service, paramObject);
                        if (ret)
                        {
                            bSuccess = true;
                            result.Data = new { Code = "", Data = "设置成功" };
                        }
                        else
                        {
                            result.Data = new { Code = "101", Data = "设置失败" };
                        }
                    }
                }
                else
                {
                    result.Data = new { Code = "102", Data = "设置失败，未找到设置参数的方法" };
                }
                //日志记录
                using (var ctx = new GlsunViewEntities())
                {
                    var d = ctx.Device.Find(did);
                    var log = ctx.DeviceOperationLog.Create();
                    var user = ctx.User.Where(u => u.ULoginName == HttpContext.User.Identity.Name).FirstOrDefault();
                                
                    //基本信息
                    log.DID = d.ID;
                    log.DName = d.DName;
                    log.DAddress = d.DAddress;
                    log.SID = d.Subnet.ID;
                    log.SName = d.Subnet.SName;
                    log.SAddress = d.Subnet.SAddress;
                    log.UID = user.ID;
                    log.ULoginName = user.ULoginName;
                    log.UName = user.UName;
                    //业务信息
                    log.DOLCardSN = edfaInfo.Serial_Number;
                    log.DOLCardType = "EDFA";
                    log.DOLDeviceSlot = short.Parse(slot.ToString());
                    log.DOLOperationDetials = operation;
                    log.DOLOperationType = "板卡配置";
                    log.DOLOperationResult = bSuccess ? "成功" : "失败";
                    log.DOLOperationTime = DateTime.Now;
                    log.Remark = "";

                    ctx.DeviceOperationLog.Add(log);
                    ctx.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                result.Data = new { Code = "Exception", Data = "设置时发生异常" };
            }
            return result;
        }
        [HttpGet]
        public ActionResult RealTimeStatus(int did, int slot)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {
                Device d = null;
                using (var ctx = new GlsunViewEntities())
                {
                    d = ctx.Device.Find(did);
                }
                EDFAInfo edfaInfo = new EDFAInfo();
                TcpClientService tcp = new TcpClientService(d.DAddress, d.DPort.Value);
                EDFACommService service = new EDFACommService(tcp, slot);
                tcp.Connect();
                edfaInfo.RefreshData(service);
                result.Data = new { Code = "", Data = edfaInfo };
            }
            catch(Exception ex)
            {
                result.Data = new { Code = "Exception", Data = "" };
            }
            return result;
        }
    }
}