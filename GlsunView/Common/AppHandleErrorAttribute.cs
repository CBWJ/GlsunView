using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace GlsunView.Common
{
    public class AppHandleErrorAttribute : HandleErrorAttribute
    {
        static ILog logger = LogManager.GetLogger(typeof(AppHandleErrorAttribute));
        public override void OnException(ExceptionContext filterContext)
        {
            int code = HttpContext.Current.Response.StatusCode;
            Exception error = filterContext.Exception;
            var message = error.Message;
            //设置为true，则说明过滤器处理了该异常
            filterContext.ExceptionHandled = true;
            filterContext.Result = new ViewResult
            {
                ViewName = "Error500",
                ViewData = new ViewDataDictionary<string>(message)
            };
            logger.Error(string.Format("{0}\r\n{1}", error.Message, error.StackTrace));
        }
    }
}