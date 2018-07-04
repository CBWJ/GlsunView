using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlsunView.Infrastructure.Abstract;
using GlsunView.Domain;
using System.Web.Security;
using System.Web;

namespace GlsunView.Infrastructure.Concrete
{
    public class FormsAuthProvider : IAuthProvider
    {
        public bool Authenticate(string username, string password)
        {
            using (var context = new GlsunViewEntities())
            {
                var user = (from u in context.User
                            where u.ULoginName == username && u.UPassword == password
                            select u).FirstOrDefault();
                //用户存在
                if(user != null)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetAuthCookie(HttpResponseBase response, string username)
        {
            if (response == null)
                return;
            //创建票据
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, username,
                DateTime.Now, DateTime.Now.AddMinutes(3), true, "");
            //加密票据
            string authTicket = FormsAuthentication.Encrypt(ticket);
            //创建Cookie 
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, authTicket);
            cookie.Expires = DateTime.Now.AddMinutes(1);
            response.Cookies.Add(cookie);
        }

        public bool IsUserExisted(string username)
        {
            using (var context = new GlsunViewEntities())
            {
                var userCount = (from u in context.User
                                 where u.ULoginName == username
                                 select u).Count();
                //用户存在
                if (userCount > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsLocked(string username)
        {
            User user = null;
            using (var ctx = new GlsunViewEntities())
            {
                user = (from u in ctx.User
                        where u.ULoginName == username
                        select u).FirstOrDefault();
            }
            if(user != null)
            {
                if (user.UUserType != 0 && user.UIsLock == true)
                    return true;
            }
            return false;
        }
        public bool IsExpired(string username)
        {
            User user = null;
            using (var ctx = new GlsunViewEntities())
            {
                user = (from u in ctx.User
                        where u.ULoginName == username
                        select u).FirstOrDefault();
            }
            if (user != null)
            {
                if (user.UUserType != 0 && user.UExpireTime <= DateTime.Now)
                    return true;
            }
            return false;
        }
    }
}
