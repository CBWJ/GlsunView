using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.CommService;
using GlsunView.Models;
using System.Web.Script.Serialization;
using GlsunView.Infrastructure.Util;

namespace GlsunView.Controllers
{
    public class OEOCardController : Controller
    {
        // GET: OEOCard
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
            using (var ctx = new GlsunViewEntities())
            {
                d = ctx.Device.Find(did);
            }
            OEOInfo oeoInfo = new OEOInfo();
            TcpClientService tcp = new TcpClientService(d.DAddress, d.DPort.Value);
            OEOCommService service = new OEOCommService(tcp, slot);
            try
            {
                tcp.Connect();
                oeoInfo.RefreshData(service);
            }
            catch (Exception ex)
            {

            }
            ViewBag.Did = d.ID;
            ViewBag.EndPoint = d.DAddress + ":" + d.DPort.Value + ":" + slot.ToString();
            return View(oeoInfo);
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
            OEOInfo oeoInfo = new OEOInfo();
            OEOViewModel model = new OEOViewModel()
            {
                IP = ip,
                Port = port,
                Slot = slot
            };
            var tcp = TcpClientServicePool.GetService(ip, port);
            if (tcp != null)
            {
                OEOCommService service = new OEOCommService(tcp, slot);
                oeoInfo.RefreshData(service);
                tcp.IsBusy = false;
                model.Type = "OEO";                
                model.Status = "正常";
                model.ProductModel = "OTS-OEO";
                model.SerialNumber = oeoInfo.Serial_Number;
                model.HardwareVersion = oeoInfo.Hardware_Version;
                model.SoftwareVersion = oeoInfo.Software_Version;

                //设置OEO卡的工作模式
                List<int> workModeSet = new List<int>();
                foreach(var sfp in oeoInfo.SFPSet)
                {
                    if(sfp.Status == 1)
                    {
                        if (!workModeSet.Contains(sfp.Work_Mode))
                            workModeSet.Add(sfp.Work_Mode);
                    }
                }
                string workMode = "";
                foreach(var mode in workModeSet)
                {
                    string text = "";
                    switch (mode)
                    {
                        case 1:
                            text = "转发";
                            break;
                        case 3:
                            text = "交叉";
                            break;
                    }
                    if (!string.IsNullOrWhiteSpace(workMode))
                    {
                        workMode += "+" + text;
                    }
                    else workMode = text;
                }
                model.WorkMode = workMode;
            }
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
            try
            {
                OEOInfo oeoInfo = new OEOInfo();
                var tcp = TcpClientServicePool.GetService(ip, port);
                if (tcp == null) throw new NullReferenceException();
                OEOCommService service = new OEOCommService(tcp, slot);
                oeoInfo.RefreshData(service);
                tcp.IsBusy = false;
                result.Data = new { Code = "", Data = oeoInfo.SFPSet };
            }
            catch (Exception ex)
            {
                result.Data = new { Code = "Exception", Data = "" };
            }
            return result;
        }
        /// <summary>
        /// OEO配置
        /// </summary>
        /// <param name="modules"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public ActionResult SetConfiguration(List<SFPModule> modules, string ip, int port, int slot)
        {
            JsonResult result = new JsonResult();
            var tcp = TcpClientServicePool.GetService(ip, port);
            if (tcp != null)
            {
                try
                {
                    List<string> listException = new List<string>();
                    OEOCommService service = new OEOCommService(tcp, slot);
                    foreach(var sfp in modules)
                    {
                        if(!service.SetWorkMode(sfp.SlotPosition, sfp.Work_Mode))
                        {
                            listException.Add(string.Format("SFP模块{0}工作模式", sfp.SlotPosition));
                        }
                        if(!service.SetTxPowerControl(sfp.SlotPosition, sfp.Tx_Power_Control))
                        {
                            listException.Add(string.Format("SFP模块{0}发功率控制", sfp.SlotPosition));
                        }
                    }
                    if (listException.Count == 0)
                    {
                        result.Data = new { Code = "", Data = "设置成功" };
                    }
                    else
                    {
                        string data = "设置失败：";
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
        public ActionResult SetParam(string endpoint, SFPModule sfp, string paramName, int did)
        {
            JsonResult result = new JsonResult();
            bool bSuccess = false;
            string operation = "";
            int slot = 0;
            try
            {
                OEOInfo oeoInfo = new OEOInfo();
                var arrPoint = endpoint.Split(':');
                if (arrPoint.Length == 3 && sfp != null)
                {
                    slot = int.Parse(arrPoint[2]);
                    TcpClientService tcp = new TcpClientService(arrPoint[0], int.Parse(arrPoint[1]));
                    OEOCommService service = new OEOCommService(tcp, slot);
                    tcp.Connect();
                    oeoInfo.RefreshData(service);
                    if (paramName == "Work_Mode")
                    {
                        operation = string.Format("设置光模块{0}工作模式", sfp.SlotPosition);
                        bSuccess = service.SetWorkMode(sfp.SlotPosition, sfp.Work_Mode);
                    }
                    else if (paramName == "Tx_Power_Control")
                    {
                        operation = string.Format("设置光模块{0}发功率控制", sfp.SlotPosition);
                        bSuccess = service.SetTxPowerControl(sfp.SlotPosition, sfp.Tx_Power_Control);
                    }
                    if (bSuccess)
                    {
                        result.Data = new { Code = "", Data = "设置成功" };
                    }
                    else
                    {
                        result.Data = new { Code = "101", Data = "设置失败" };
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
                    log.DOLCardSN = oeoInfo.Serial_Number;
                    log.DOLCardType = "OEO";
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
                string key = string.Format("oeo_info_{0}", did);
                var info =  MemoryCacheHelper.GetCacheItem<OEOInfo>(key,
                    () =>
                    {
                        Device d = null;
                        using (var ctx = new GlsunViewEntities())
                        {
                            d = ctx.Device.Find(did);
                        }
                        OEOInfo oeoInfo = new OEOInfo();
                        //TcpClientService tcp = new TcpClientService(d.DAddress, d.DPort.Value);
                        var tcp = TcpClientServicePool.GetService(d.DAddress, d.DPort.Value);
                        if (tcp == null) throw new NullReferenceException();
                        OEOCommService service = new OEOCommService(tcp, slot);
                        oeoInfo.RefreshData(service);
                        tcp.IsBusy = false;
                        return oeoInfo;
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