using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace GlsunView.Infrastructure.Concrete
{
    /// <summary>
    /// 全局认证过渡器
    /// </summary>
    public class GlobalAuthenticationFilter : IAuthenticationFilter
    {
        /// <summary>
        /// 进行用户认证，设置Result将调用OnAuthenticationChallenge方法，否则调用请求的动作方法
        /// </summary>
        /// <param name="filterContext">派生于ControllerContext</param>
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            throw new NotImplementedException();
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            throw new NotImplementedException();
        }
    }
}
