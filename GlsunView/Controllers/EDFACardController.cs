using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.CommService;
using GlsunView.Models;
using GlsunView.Infrastructure.Util;
using System.Web.Script.Serialization;
using System.ComponentModel;
using System.Text;

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
        /// <summary>
        /// 设备视图
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public ActionResult Details(string ip, int port, int slot)
        {
            EDFAInfo edfaInfo = new EDFAInfo();
            var tcp = TcpClientServicePool.GetService(ip, port);
            if (tcp != null)
            {
                EDFACommService service = new EDFACommService(tcp, slot);
                edfaInfo.RefreshData(service);
                tcp.IsBusy = false;
            }
            EDFAViewModel edfa = new EDFAViewModel
            {
                IP = ip,
                Port = port,
                Slot = slot,
                Type = edfaInfo.Device_Type,
                WorkMode = edfaInfo.Work_Mode,
                Status = "正常",
                MaxOutput = edfaInfo.PUMP_Power,
                MaxGain = edfaInfo.Current_Gain,
                ProductModel = "OTS-"+ edfaInfo.Device_Type,
                SerialNumber = edfaInfo.Serial_Number,
                HardwareVersion = edfaInfo.Hardware_Version,
                SoftwareVersion = edfaInfo.Software_Version
            };
            return View(edfa);
        }
        /// <summary>
        /// 设备视图更新配置信息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateConfig(string ip, int port, int slot)
        {
            JsonResult result = new JsonResult();
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {
                EDFAInfo edfaInfo = new EDFAInfo();
                var tcp = TcpClientServicePool.GetService(ip, port);
                if (tcp == null) throw new NullReferenceException();
                EDFACommService service = new EDFACommService(tcp, slot);
                edfaInfo.RefreshData(service);
                tcp.IsBusy = false;
                result.Data = new { Code = "", Data = edfaInfo };
            }
            catch (Exception ex)
            {
                result.Data = new { Code = "Exception", Data = "" };
            }
            return result;
        }
        public ActionResult SetConfiguration(EDFAInfo edfaInfo, string ip, int port, int slot)
        {
            JsonResult result = new JsonResult();
            var tcp = TcpClientServicePool.GetService(ip, port);
            if(tcp != null)
            {
                try
                {
                    EDFACommService service = new EDFACommService(tcp, slot);
                    List<string> listException = new List<string>();
                    if (!service.SetWorkMode(edfaInfo.Work_Mode))
                    {
                        listException.Add("工作模式");
                    }
                    if (!service.SetGainSetting(edfaInfo.Gain_Setting))
                    {
                        listException.Add("增益");
                    }
                    if(listException.Count == 0)
                    {
                        result.Data = new { Code = "", Data = "配置成功" };
                    }
                    else
                    {
                        string data = "配置失败：";
                        foreach(var e in listException)
                        {
                            data += e + " ";
                        }
                        result.Data = new { Code = "Exception", Data = data };
                    }
                }
                catch(Exception ex)
                {
                    result.Data = new { Code = "Exception", Data = ex.Message };
                }
                finally
                {
                    tcp.IsBusy = false;
                }
            }
            else
            {
                result.Data = new { Code = "Exception", Data = "获取TCP连接失败" };
            }
            return result;
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
                string key = string.Format("edfa_info_{0}", did);
                var info = MemoryCacheHelper.GetCacheItem<EDFAInfo>(key,
                    () =>
                    {
                        Device d = null;
                        using (var ctx = new GlsunViewEntities())
                        {
                            d = ctx.Device.Find(did);
                        }
                        EDFAInfo edfaInfo = new EDFAInfo();
                        //TcpClientService tcp = new TcpClientService(d.DAddress, d.DPort.Value);
                        var tcp = TcpClientServicePool.GetService(d.DAddress, d.DPort.Value);
                        if (tcp == null) throw new NullReferenceException();
                        EDFACommService service = new EDFACommService(tcp, slot);
                        edfaInfo.RefreshData(service);
                        tcp.IsBusy = false;
                        return edfaInfo;
                    },
                    null, DateTime.Now.AddSeconds(2));
                result.Data = new { Code = "", Data = info };
            }
            catch(Exception ex)
            {
                result.Data = new { Code = "Exception", Data = "" };
            }
            return result;
        }
    }
}