using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Filters;
using GlsunView.Infrastructure.Util;
using System.Web.Mvc;
using System.Web.Routing;

namespace GlsunView.Common
{
    /// <summary>
    /// 全局认证过滤器
    /// </summary>
    public class GlobalAuthenticationFilter : IAuthenticationFilter
    {
        /// <summary>
        /// set the result,OnAuthenticationChallenge will be invoked, else the action method will be invoked.
        /// </summary>
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            //AuthenticationContext.Result：设置表示认证质疑的ActionResult
            if (!LisenceHelper.IsLisenceValid())
            {
                if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() == "lisence" &&
                filterContext.ActionDescriptor.ActionName.ToLower() == "exception")
                {

                }
                else
                {
                    filterContext.Result = new LisenceExceptionResult();
                }
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if(filterContext.Result is LisenceExceptionResult)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    {"controller", "Lisence" },
                    {"action", "Exception" }
                });
            }
        }
    }
}