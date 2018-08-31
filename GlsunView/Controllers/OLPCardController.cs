using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.CommService;
using GlsunView.Models;
using System.Web.Script.Serialization;
using System.Reflection;
using System.ComponentModel;
using GlsunView.Infrastructure.Util;

namespace GlsunView.Controllers
{
    public class OLPCardController : Controller
    {
        // GET: OLPCard
        public ActionResult Index(int did, int slot)
        {
            Device d = null;
            using (var ctx = new GlsunViewEntities())
            {
                d = ctx.Device.Find(did);
            }
            OLPInfo olpInfo = new OLPInfo();
            TcpClientService tcp = new TcpClientService(d.DAddress, d.DPort.Value);
            OLPCommService service = new OLPCommService(tcp, slot);
            try
            {
                tcp.Connect();
                olpInfo.RefreshData(service);
            }
            catch (Exception ex)
            {

            }
            ViewBag.Did = d.ID;
            ViewBag.EndPoint = d.DAddress + ":" + d.DPort.Value + ":" + slot.ToString();
            return View(olpInfo);
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
            OLPInfo olpInfo = new OLPInfo();
            OLPViewModel model = new OLPViewModel()
            {
                IP = ip,
                Port = port,
                Slot = slot
            };
            
            var tcp = TcpClientServicePool.GetService(ip, port);
            if (tcp != null)
            {
                try
                {
                    OLPCommService service = new OLPCommService(tcp, slot);
                    olpInfo.RefreshData(service);
                    model.Type = olpInfo.Card_Type;
                    model.Status = "正常";
                    model.ProductModel = "OTS-" + model.Type;
                    model.SerialNumber = olpInfo.Serial_Number;
                    model.HardwareVersion = olpInfo.Hardware_Version;
                    model.SoftwareVersion = olpInfo.Software_Version;
                    model.WorkMode = olpInfo.Work_Mode;
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    tcp.IsBusy = false;
                }
            }
            return View(model);
        }
        /// <summary>
        /// 路由视图OLP部分
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public ActionResult RouteView(string ip, int port, int slot)
        {
            OLPInfo olpInfo = new OLPInfo();
            OLPViewModel model = new OLPViewModel()
            {
                IP = ip,
                Port = port,
                Slot = slot
            };
            
            return View(model);
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
            OLPInfo olpInfo = new OLPInfo();
            var tcp = TcpClientServicePool.GetService(ip, port);
            if (tcp != null)
            {
                try
                {
                    OLPCommService service = new OLPCommService(tcp, slot);
                    olpInfo.RefreshData(service);
                    result.Data = new { Code = "", Data = olpInfo };
                }
                catch (Exception ex)
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
        /// <summary>
        /// OLP配置
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public ActionResult SetConfiguration(OLPInfo info, string ip, int port, int slot)
        {
            JsonResult result = new JsonResult();
            var tcp = TcpClientServicePool.GetService(ip, port);
            if (tcp != null)
            {
                try
                {
                    List<string> listException = new List<string>();
                    OLPCommService service = new OLPCommService(tcp, slot);
                    List<string> listMethod = new List<string>();
                    foreach (var prop in info.GetType().GetProperties().OrderBy(p => p.Name))
                    {
                        string name = prop.Name;
                        object value = prop.GetValue(info);
                        //设置方法名
                        string methodName = "Set" + name.Replace("_", "");
                        var methodInfo = service.GetType().GetMethod(methodName);
                        if (methodInfo != null)
                        {
                            listMethod.Add(methodInfo.Name);
                            string operation = "";
                            //获取设置项说明
                            object[] arrDescription = methodInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                            if (arrDescription != null && arrDescription.Length > 0)
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
                            foreach (var e in paramInfo)
                            {
                                paramObject[0] = Convert.ChangeType(value, e.ParameterType);
                            }

                            var ret = (bool)methodInfo.Invoke(service, paramObject);
                            if (!ret)
                            {
                                listException.Add(operation);
                            }
                        }
                    }
                    if (listException.Count == 0)
                    {
                        result.Data = new { Code = "", Data = "配置成功" };
                    }
                    else
                    {
                        string data = "配置失败：";
                        foreach (var e in listException)
                        {
                            data += e + " ";
                        }
                        result.Data = new { Code = "Exception", Data = data };
                    }
                }
                catch (Exception ex)
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
        public ActionResult SetParam(string endpoint, string name, string value, int did)
        {
            JsonResult result = new JsonResult();
            bool bSuccess = false;
            string operation = "";
            int slot = 0;
            try
            {
                OLPInfo olpInfo = new OLPInfo();
                var arrPoint = endpoint.Split(':');
                if (arrPoint.Length == 3)
                {
                    slot = int.Parse(arrPoint[2]);
                    TcpClientService tcp = new TcpClientService(arrPoint[0], int.Parse(arrPoint[1]));
                    tcp.Connect();
                    OLPCommService service = new OLPCommService(tcp, slot);
                    olpInfo.RefreshData(service);
                    string methodName = "Set" + name.Replace("_", "");
                    var methodInfo = service.GetType().GetMethod(methodName);
                    if(methodInfo != null)
                    {
                        //获取设置项
                        object[] arrDescription = methodInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (arrDescription != null && arrDescription.Length > 0)
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
                        foreach(var e in paramInfo)
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
                    else
                    {
                        result.Data = new { Code = "102", Data = "设置失败，未找到设置参数的方法" };
                    }
                    tcp.Dispose();
                }
                else
                {
                    result.Data = new { Code = "100", Data = "设置失败，请求参数错误" };
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
                    log.DOLCardSN = olpInfo.Serial_Number;
                    log.DOLCardType = "OLP";
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
            catch (Exception ex)
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
                string key = string.Format("olp_info_{0}", did);
                var info = MemoryCacheHelper.GetCacheItem<OLPInfo>(key,
                    () =>
                    {
                        Device d = null;
                        using (var ctx = new GlsunViewEntities())
                        {
                            d = ctx.Device.Find(did);
                        }
                        OLPInfo olpInfo = new OLPInfo();
                        //TcpClientService tcp = new TcpClientService(d.DAddress, d.DPort.Value);
                        var tcp = TcpClientServicePool.GetService(d.DAddress, d.DPort.Value);
                        if (tcp == null) throw new NullReferenceException();
                        OLPCommService service = new OLPCommService(tcp, slot);
                        olpInfo.RefreshData(service);
                        tcp.IsBusy = false;
                        return olpInfo;
                    },
                    null, DateTime.Now.AddSeconds(2));
                result.Data = new { Code = "", Data = info };
            }
            catch (Exception ex)
            {
                result.Data = new { Code = "Exception", Data = "" };
            }
            return result;
        }
    }
}