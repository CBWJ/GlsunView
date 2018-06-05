using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using System.Collections;
using GlsunView.Infrastructure.Concrete;

namespace GlsunView.Controllers
{
    public class ShareListController : Controller
    {
        /// <summary>
        /// 设置用户操作权限的数据
        /// </summary>
        public virtual void SetAuthorityData()
        {
            IEnumerable<v_UserModuleAuthority> userModuleAuth = null;
            string controllerName = (string)ControllerContext.RouteData.Values["controller"];
            string actionName = (string)ControllerContext.RouteData.Values["action"];
            string moduleUrl = controllerName + "/" + actionName;
            //拓扑管理用子级操作权限
            if(moduleUrl.ToLower() == "topology/index")
            {
                moduleUrl = "Subnet/Index";
            }
            using (var ctx = new GlsunViewEntities())
            {
                var loginUser = (from u in ctx.User
                                 where u.ULoginName == HttpContext.User.Identity.Name
                                 select u).FirstOrDefault();
                if (loginUser.UUserType == 0)
                {
                    var module = ctx.Module.Where(m => m.MUrl == moduleUrl).FirstOrDefault();
                    if (module != null)
                    {
                        var subSet = (from m in ctx.Module
                                      join ma in ctx.ModuleAuthority
                                      on m.ID equals ma.MID
                                      join a in ctx.Authority
                                      on ma.AID equals a.ID
                                      where m.ID == module.ID
                                      select new
                                      {
                                          AName = a.AName,
                                          AIcon = a.AIcon,
                                          ACode = a.ACode,
                                          AClassName = a.AClassName,
                                          AShowNumber = a.AShowNumber
                                      }).ToList();
                        userModuleAuth = from n in subSet
                                         orderby n.AShowNumber
                                         select new v_UserModuleAuthority
                                         {
                                             AName = n.AName,
                                             AIcon = n.AIcon,
                                             ACode = n.ACode,
                                             AClassName = n.AClassName
                                         };
                    }
                }
                else
                {
                    userModuleAuth = ctx.v_UserModuleAuthority
                        .Where(uma => uma.UID == loginUser.ID && uma.MUrl == moduleUrl)
                        .OrderBy(uma => uma.AShowNumber)
                        .ToList()
                        .Distinct(new UserModuleAuthorityComparer());
                }
            }
            ViewBag.UserModuleAuth = userModuleAuth;
        }
    }
}