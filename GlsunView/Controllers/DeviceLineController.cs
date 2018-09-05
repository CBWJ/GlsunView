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

        public ActionResult Edit(int id)
        {
            DeviceLine line = null;
            int RGID = 0;
            using (var ctx = new GlsunViewEntities())
            {
                line = ctx.DeviceLine.Find(id);
                if(line.RID.HasValue)
                {
                    var route = ctx.Route.Find(line.RID);
                    if(route != null)
                    {
                        RGID = route.RGID.Value;
                    }
                }
            }
            ViewBag.RGID = RGID;
            return View(line);
        }
        [HttpPost]
        public ActionResult Edit(DeviceLine line)
        {
            var json = new JsonResult();
            User loginUser = null;
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                 where u.ULoginName == HttpContext.User.Identity.Name
                                 select u).FirstOrDefault();
                    var route = ctx.Route.Find(line.RID);
                    var modifyLine = ctx.DeviceLine.Find(line.ID);
                    modifyLine.DLName = route.RName;
                    modifyLine.RID = line.RID;
                    modifyLine.EditorID = loginUser.ID;
                    modifyLine.EditingTime = DateTime.Now;
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = "", Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = "", Message = ex.Message };
            }
            return json;
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
                    Device dA = ctx.Device.Find(line.NodeIDA);
                    Device dB = ctx.Device.Find(line.NodeIDZ);
                    /*Route route = ctx.Route.Where(r => (r.RAMFID == dA.MFID && r.RBMFID == dB.MFID) || 
                                                        (r.RAMFID == dB.MFID && r.RBMFID == dA.MFID)).FirstOrDefault();
                    if(route != null)
                    {
                        newLine.DLName = route.RName;
                    }
                    else
                    {
                        newLine.DLName = string.Format("{0}-{1}", dA.DName, dB.DName);
                    }*/
                    newLine.DIDA = line.NodeIDA;
                    newLine.DIDB = line.NodeIDZ;
                    newLine.CreatorID = loginUser.ID;
                    newLine.CreationTime = DateTime.Now;

                    ctx.DeviceLine.Add(newLine);
                    ctx.SaveChanges();
                    line.ID = newLine.ID;
                    //line.Name = newLine.DLName;
                    
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