using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.CommService;
using GlsunView.Models;
using System.Web.Script.Serialization;

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
                Device d = null;
                using (var ctx = new GlsunViewEntities())
                {
                    d = ctx.Device.Find(did);
                }
                OEOInfo oeoInfo = new OEOInfo();
                TcpClientService tcp = new TcpClientService(d.DAddress, d.DPort.Value);
                OEOCommService service = new OEOCommService(tcp, slot);
                tcp.Connect();
                oeoInfo.RefreshData(service);
                result.Data = new { Code = "", Data = oeoInfo };
            }
            catch (Exception ex)
            {
                result.Data = new { Code = "Exception", Data = "" };
            }
            return result;
        }
    }
}