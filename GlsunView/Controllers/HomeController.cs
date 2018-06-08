using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using System.Text;

namespace GlsunView.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            User loginUser = null;
            using (var context = new GlsunViewEntities())
            {
                loginUser = (from u in context.User
                             where u.ULoginName == HttpContext.User.Identity.Name
                             select u).FirstOrDefault();
                if (loginUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            //传入到视图的信息：用户信息，模块信息
            return View(loginUser);
        }
        
        public PartialViewResult Menu()
        {
            List<Module> modules = null;
            using (var context = new GlsunViewEntities())
            {
                var loginUser = (from u in context.User
                             where u.ULoginName == HttpContext.User.Identity.Name
                             select u).FirstOrDefault();
                //管理员
                if (loginUser.UUserType == 0)
                {
                    modules = (from m in context.Module
                               where m.IsEnabled == true && m.MType == "menu" && m.MName != "拓扑操作"
                               select m).ToList();
                }
                else
                {
                    var subSet = (from um in context.v_UserModule
                                  where um.UID == loginUser.ID
                                  select um.MID).ToList().Distinct();
                    modules = context.Module.Where(m => subSet.Contains(m.ID) && m.MType == "menu" && m.MName != "拓扑操作").ToList();
                }
                var topoModule = modules.Where(m => m.MName == "拓扑管理").FirstOrDefault();
                if (topoModule != null)
                {
                    var netId = 100000;
                    var deviceId = 200000;
                    foreach (var net in context.Subnet.OrderBy(s => s.ID))
                    {
                        var m = new Module
                        {
                            ID = netId + net.ID,
                            MParentID = topoModule.ID,
                            MLevel = 1,
                            MIconType = "image",
                            MIcon = "image/folder.png",
                            MName = net.SName,
                            MUrl = string.Format("Subnet/Index/{0}", net.ID)
                        };
                        modules.Add(m);
                        foreach (var device in context.Device.Where(d => d.SID == net.ID).OrderBy(d => d.ID))
                        {
                            var md = new Module
                            {
                                ID = deviceId + device.ID,
                                MParentID = m.ID,
                                MLevel = 2,
                                MIconType = "image",
                                MIcon = "image/document_file.png",
                                MName = device.DName,
                                MUrl = string.Format("Device/Index/{0}", device.ID)
                            };
                            modules.Add(md);
                        }
                    } 
                }
            }
            return PartialView(modules);
        }
        [HttpGet]
        public ActionResult GetRealTimeAlarmCount()
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            int criticalCount = 0;
            int majorCount = 0;
            int minorCount = 0;
            int warnCount = 0;
            int normalCount = 0;
            //统计未确认告警
            using(var ctx = new GlsunViewEntities())
            {
                criticalCount = ctx.AlarmInformation.Where(a => a.AIConfirm == false && a.AILevel == "CRITICAL").Count();
                majorCount = ctx.AlarmInformation.Where(a => a.AIConfirm == false && a.AILevel == "MAJOR").Count();
                minorCount = ctx.AlarmInformation.Where(a => a.AIConfirm == false && a.AILevel == "MINOR").Count();
                warnCount = ctx.AlarmInformation.Where(a => a.AIConfirm == false && a.AILevel == "WARN").Count();
                normalCount = ctx.AlarmInformation.Where(a => a.AIConfirm == false && a.AILevel == "NORMAL").Count();
            }
            json.Data = new
            {
                critical = criticalCount,
                major = majorCount,
                minor = minorCount,
                warn = warnCount,
                normal = normalCount
            };
            return json;
        }

        public ActionResult Home()
        {
            return View();
        }
    }
}