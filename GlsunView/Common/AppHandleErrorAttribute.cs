﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlsunView.Common
{
    public class AppHandleErrorAttribute : HandleErrorAttribute
    {
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
        }
    }
}