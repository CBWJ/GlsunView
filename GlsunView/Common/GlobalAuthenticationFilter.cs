using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Filters;
using GlsunView.Infrastructure.Util;

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
            var a = filterContext;
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            string msg="";
            //if(!LisenceHelper.IsLisenceValid(out msg))
            //{
            //    var a = "";
            //}
        }
    }
}