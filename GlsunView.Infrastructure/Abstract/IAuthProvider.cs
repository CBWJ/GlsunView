﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GlsunView.Infrastructure.Abstract
{
    /// <summary>
    /// 用户认证接口
    /// </summary>
    public interface IAuthProvider
    {
        bool Authenticate(string username, string password);
        void SetAuthCookie(HttpResponseBase response, string username);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        bool IsUserExisted(string username);
        bool IsLocked(string username);
        bool IsExpired(string username);
    }
}
