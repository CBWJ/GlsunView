﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Common;

namespace GlsunView
{
    public class FilterConfig
    {
        /// <summary>
        /// 注册全局过滤器
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AppHandleErrorAttribute());
            filters.Add(new GlobalAuthenticationFilter());
            filters.Add(new AuthorizeAttribute());
        }
    }
}