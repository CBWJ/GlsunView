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
    public class NMUCardController : Controller
    {
        // GET: NMUCard
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <returns></returns>
        public ActionResult Index(int id)
        {
            Device d = null;
            using (var ctx = new GlsunViewEntities())
            {
                d = ctx.Device.Find(id);
            }
            //IP地址->连接服务->实例化一个状态类->填充状态
            TcpClientService tcp = new TcpClientService(d.DAddress, d.DPort.Value);
            NMUCommService service = new NMUCommService(tcp);
            NMUInfo nmuInfo = new NMUInfo();
            try
            {
                tcp.Connect();
                nmuInfo.RefreshStatus(service);
            }
            catch(Exception ex)
            {

            }
            tcp.Dispose();
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //ViewBag.DeviceInfo = serializer.Serialize(deviceView);
            ViewBag.DID = d.ID;
            return View(nmuInfo);
        }
        [HttpGet]
        public ActionResult RealTimeStatus(int did)
        {
            var ret = new JsonResult();
            ret.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {
                Device d = new Device();
                using (var ctx = new GlsunViewEntities())
                {
                    d = ctx.Device.Find(did);
                }
                //IP地址->连接服务->实例化一个状态类->填充状态
                TcpClientService tcp = new TcpClientService(d.DAddress, d.DPort.Value);
                NMUCommService service = new NMUCommService(tcp);
                NMUInfo nmuInfo = new NMUInfo();
                tcp.Connect();
                nmuInfo.RefreshStatus(service);
                tcp.Dispose();
                ret.Data = new { Code = "", Data = nmuInfo };
            }
            catch(Exception ex)
            {
                ret.Data = new { Code = "Exception", Data = "" };
            }
            return ret;
        }
    }
}