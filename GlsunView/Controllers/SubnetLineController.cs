using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using GlsunView.Domain;
using GlsunView.Models;
using System.Web;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using GlsunView.Infrastructure.Util;
using GlsunView.Infrastructure.Concrete;

namespace GlsunView.Controllers
{
    public class SubnetLineController : Controller
    {
        private TopologyLogger _topoLogger = new TopologyLogger();
        //private string UserName
        //{
        //    get
        //    {
        //        var content = ControllerContext.Request.Properties["MS_HttpContext"] as HttpContextBase;
        //        var encyptticket = content.Request.Cookies[".ASPXAUTH"].Value;
        //        var ticket = FormsAuthentication.Decrypt(encyptticket);
        //        return ticket.Name;
        //    }
        //}
        // GET: api/SubnetLine
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SubnetLine/5
        public string Get(int id)
        {
            return "value";
        }
        [HttpPost]
        public string DeleteLine(int id)
        {
            object result = null;
            User loginUser = null;
            SubnetLine line = null;
            string details = "";
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    var lineDel = (from l in ctx.SubnetLine
                                   where l.ID == id
                                   select l).FirstOrDefault();
                    line = lineDel.CopyProperty();
                    ctx.SubnetLine.Remove(lineDel);
                    ctx.SaveChanges();
                    var sA = ctx.Subnet.Find(line.SIDA);
                    var sB = ctx.Subnet.Find(line.SIDB);
                    details = string.Format("{0}-{1}", sA.SName, sB.SName);
                }
                result = new { Code = "", Data = id, Message = "保存成功" };
                //日志记录
                _topoLogger.Record(loginUser, "删除子网连线", details, "成功", "", line.ID, line.SLName, "子网连线");
            }
            catch (Exception ex)
            {
                result = new { Code = "Exception", Data = id, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "删除子网连线", details, "失败", string.Format("发生异常：{0}", ex.Message), line.ID, line.SLName, "子网连线");
            }

            return new JavaScriptSerializer().Serialize(result);
        }

        [HttpPost]
        public string AddLine(TopologyLine line)
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
                    var newLine = ctx.SubnetLine.Create();

                    newLine.SIDA = line.NodeIDA;
                    newLine.SIDB = line.NodeIDZ;
                    newLine.CreatorID = loginUser.ID;
                    newLine.CreationTime = DateTime.Now;

                    ctx.SubnetLine.Add(newLine);
                    ctx.SaveChanges();
                    line.ID = newLine.ID;

                    var sA = ctx.Subnet.Find(line.NodeIDA);
                    var sB = ctx.Subnet.Find(line.NodeIDZ);
                    details = string.Format("{0}-{1}", sA.SName, sB.SName);
                }
                result = new { Code = "", Data = line, Message = "保存成功" };
                //日志记录
                _topoLogger.Record(loginUser, "添加子网连线", details, "成功", "", line.ID, line.Name, "子网连线");
            }
            catch (Exception ex)
            {
                result = new { Code = "Exception", Data = line, Message = ex.Message };
                //日志记录
                _topoLogger.Record(loginUser, "添加子网连线", details, "失败", string.Format("发生异常：{0}", ex.Message), line.ID, line.Name, "子网连线");
            }

            return new JavaScriptSerializer().Serialize(result);
        }
    }
}
