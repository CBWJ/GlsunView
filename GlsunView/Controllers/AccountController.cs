using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Infrastructure.Abstract;
using GlsunView.Infrastructure.Concrete;
using GlsunView.Infrastructure.Util;
using System.Web.Security;

namespace GlsunView.Controllers
{
    public class AccountController : Controller
    {
        private IAuthProvider _authProvider = new FormsAuthProvider();
        private IUserlogger _userLogger = new Userlogger();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        //GET
        public ActionResult Login()
        {
            User user = new User();
            //获取Cookie
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                if (ticket.Expired == false)
                {
                    user.ULoginName = ticket.Name;
                    user.UPassword = "***";
                }
                else
                {
                    ViewBag.LoginMsg = "身份已过期，请重新登陆";
                    ViewBag.IsValid = false;
                }
            }
            return View(user);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(User user, string rememberUser)
        {
            bool bRememberUser = rememberUser == "checked";
            //获取Cookie
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                if (!ticket.Expired && ticket.Name == user.ULoginName)
                {
                    //再次检查用户有效性
                    if (_authProvider.IsUserExisted(user.ULoginName))
                    {
                        return RedirectToAction("index", "Home");
                    }
                    else
                    {
                        ViewBag.LoginMsg = "账号不存在！";
                        ViewBag.IsValid = false;
                        return View(user);
                    }
                }
            }
            if (!_authProvider.IsUserExisted(user.ULoginName))
            {
                ViewBag.LoginMsg = "账号不存在！";
                ViewBag.IsValid = false;
                return View(user);
            }
            
            if (_authProvider.Authenticate(user.ULoginName, user.UPassword))
            {
                if (_authProvider.IsLocked(user.ULoginName))
                {
                    _userLogger.RecordLogin(user.ULoginName, "登录", "失败", "账号已锁定");
                    ViewBag.LoginMsg = "账号已锁定，请联系管理员！";
                    ViewBag.IsValid = false;
                    return View(user);
                }
                if (_authProvider.IsExpired(user.ULoginName))
                {
                    _userLogger.RecordLogin(user.ULoginName, "登录", "失败", "账号已过期");
                    ViewBag.LoginMsg = "账号已过期，请联系管理员！";
                    ViewBag.IsValid = false;
                    return View(user);
                }
                if (bRememberUser)
                {
                    _authProvider.SetAuthCookie(Response, user.ULoginName);
                    _userLogger.RecordLogin(user.ULoginName, "登录", "成功", "记住账号");
                }
                else
                {
                    /*为用户名创建一个身份验证票据，并将其添加到响应的Cookie中
                * SetAuthCookie的第一个参数为已验证的用户的名称。 
                    *SetAuthCookie的第二个参数为true时代表创建持久Cookie（跨浏览器会话保存的 Cookie），为false则关闭浏览器后要重新验证身份
                */
                    FormsAuthentication.SetAuthCookie(user.ULoginName, false);
                    _userLogger.RecordLogin(user.ULoginName, "登录", "成功", "");
                }
                return RedirectToAction("index", "Home");
            }
            else
            {
                _userLogger.RecordLogin(user.ULoginName, "登录", "失败", "账号或密码错误");
                ViewBag.LoginMsg = "账号或密码错误";
                ViewBag.IsValid = false;
                return View(user);
            }
        }

        public ActionResult Logout()
        {
            _userLogger.RecordLogin(HttpContext.User.Identity.Name, "注销", "成功", "");
            Session["User"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("index", "Home");
        }

        public ActionResult ChangePassword()
        {
            User loginUser = null;
            using (var ctx = new GlsunViewEntities())
            {
                loginUser = (from u in ctx.User
                                 where u.ULoginName == HttpContext.User.Identity.Name
                                 select u).FirstOrDefault();
            }
            return View(loginUser);
        }
        [HttpPost]
        public ActionResult ChangePassword(User user)
        {
            var json = new JsonResult();
            var bRecord = false;
            User loginUser = null;
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                     where u.ID == user.ID
                                     select u).FirstOrDefault();
                    if (loginUser != null)
                    {
                        if (loginUser.UPassword != user.UPassword)
                        {
                            user.ULoginName = loginUser.ULoginName;
                            bRecord = true;
                        }
                        loginUser.UName = user.UName;
                        loginUser.UPassword = user.UPassword;
                    }          
                    ctx.SaveChanges();
                }
                if(bRecord)
                    _userLogger.RecordModify(loginUser, user, "修改密码", "", "成功", "");
                //删除Cookie
                FormsAuthentication.SignOut();
                json.Data = new { Code = "", Data = user, Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = user, Message = ex.Message };
            }
            return json;
        }
        [HttpPost]
        public string IsValidPassword(string password)
        {
            User loginUser = null;
            using (var ctx = new GlsunViewEntities())
            {
                loginUser = (from u in ctx.User
                             where u.ULoginName == HttpContext.User.Identity.Name
                             && u.UPassword == password
                             select u).FirstOrDefault();
            }
            bool result = false;
            if (loginUser != null)
            {
                result = true;
            }
            return result.ToString().ToLower();
        }
    }
}