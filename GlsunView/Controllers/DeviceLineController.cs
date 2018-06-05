using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Models;
using GlsunView.Domain;
using System.Web.Script.Serialization;
using GlsunView.Infrastructure.Util;
using GlsunView.Infrastructure.Concrete;

namespace GlsunView.Controllers
{
    public class DeviceLineController : Controller
    {
        private TopologyLogger _topoLogger = new TopologyLogger();
        // GET: DeviceLine
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string Add(TopologyLine line)
        {
            object result = null;
            User loginUser = null;
            string details = "";
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    var newLine = ctx.DeviceLine.Create();

                    newLine.DIDA = line.NodeIDA;
                    newLine.DIDB = line.NodeIDZ;
                    newLine.CreatorID = loginUser.ID;
                    newLine.CreationTime = DateTime.Now;

                    ctx.DeviceLine.Add(newLine);
                    ctx.SaveChanges();
                    line.ID = newLine.ID;

                    var dA = ctx.Device.Find(line.NodeIDA);
                    var dB = ctx.Device.Find(line.NodeIDZ);
                    details = string.Format("{0}-{1}", dA.DName, dB.DName);
                }
                result = new { Code = "", Data = line, Message = "保存成功" };
                //日志记录
                _topoLogger.Record(loginUser, "添加设备连线", details, "成功", "", line.ID, line.Name, "设备连线");
            }
            catch (Exception ex)
            {
                result = new { Code = "Exception", Data = line, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "添加设备连线", details, "失败", string.Format("发生异常：{0}", ex.Message), line.ID, line.Name, "设备连线");
            }

            return new JavaScriptSerializer().Serialize(result);
        }

        [HttpPost]
        public string Delete(int id)
        {
            object result = null;
            User loginUser = null;
            string details = "";
            DeviceLine line = null;
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    var deviceLineDel = ctx.DeviceLine.Find(id);
                    line = deviceLineDel.CopyProperty();
                    ctx.DeviceLine.Remove(deviceLineDel);
                    ctx.SaveChanges();
                    var dA = ctx.Device.Find(line.DIDA);
                    var dB = ctx.Device.Find(line.DIDB);
                    details = string.Format("{0}-{1}", dA.DName, dB.DName);
                }
                result = new { Code = "", Data = id, Message = "删除成功" };
                //日志记录
                _topoLogger.Record(loginUser, "删除设备连线", details, "成功", "", line.ID, line.DLName, "设备连线");
            }
            catch (Exception ex)
            {
                result = new { Code = "Exception", Data = id, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "删除设备连线", details, "失败", string.Format("发生异常：{0}", ex.Message), line.ID, line.DLName, "设备连线");
            }

            return new JavaScriptSerializer().Serialize(result);
        }
    }
}