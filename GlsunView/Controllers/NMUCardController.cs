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
            return View(nmuInfo);
        }
    }
}