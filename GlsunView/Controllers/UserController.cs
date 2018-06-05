using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;
using System.Transactions;
using GlsunView.Infrastructure.Util;
using GlsunView.Infrastructure.Abstract;
using GlsunView.Infrastructure.Concrete;

namespace GlsunView.Controllers
{
    public class UserController : ShareListController
    {
        private IUserlogger _userLogger = new Userlogger();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int page = 1, int pageSize = 10)
        {
            IEnumerable<v_User> users = null;
            int itemCount = 0;
            using (var ctx = new GlsunViewEntities())
            {
                itemCount = ctx.User.Count();
                users = ctx.v_User.OrderBy(u => u.ID)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            }
            PagingInfo pagingInfo = new PagingInfo
            {
                TotalItems = itemCount,
                CurrentPage = page,
                ItemsPerPage = pageSize,
                ShowPageCount = 5
            };
            ViewBag.PagingInfo = pagingInfo;
            SetAuthorityData();
            return View(users);
        }
        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            GlsunView.Domain.User module = null;
            using (var ctx = new GlsunViewEntities())
            {
                module = (from u in ctx.User
                          where u.ID == id
                          select u).FirstOrDefault();
                ViewBag.RoleList = ctx.Role.ToList();
                ViewBag.UserRole = JsonHelper.getJsonByObject(
                        (from ur in ctx.v_UserRole
                         where ur.UID == id
                         select ur).ToList());
            }
            ViewBag.Action = "Details";
            return View("Create", module);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            GlsunView.Domain.User user = new User();
            user.UIsLock = false;
            user.UUserType = 1;
            user.UExpireTime = DateTime.Now.AddYears(10);
            using (var ctx = new GlsunViewEntities())
            {
                ViewBag.RoleList = ctx.Role.ToList();
            }
            ViewBag.Action = "Create";
            return View(user);
        }

        // POST: User/Create
        [HttpPost]
        public JsonResult Create(User user, List<int> SelectedRoleId)
        {
            var json = new JsonResult();
            try
            {
                var userId = 0;
                using (var ctx = new GlsunViewEntities())
                {
                    var loginUser = (from u in ctx.User
                                        where u.ULoginName == HttpContext.User.Identity.Name
                                        select u).FirstOrDefault();
                    if (loginUser != null)
                    {
                        user.CreatorID = loginUser.ID;
                        user.CreationTime = DateTime.Now;
                    }

                    ctx.User.Add(user);
                    ctx.SaveChanges();
                    userId = user.ID;
                }
                if (SelectedRoleId != null)
                {
                    using (var ctx = new GlsunViewEntities())
                    {
                        var loginUser = (from u in ctx.User
                                         where u.ULoginName == HttpContext.User.Identity.Name
                                         select u).FirstOrDefault();
                        foreach (var id in SelectedRoleId)
                        {
                            UserRole userRole = new UserRole
                            {
                                UID = userId,
                                RID = id,
                                CreatorID = loginUser.ID,
                                CreationTime = DateTime.Now
                            };
                            ctx.UserRole.Add(userRole);
                        }
                        ctx.SaveChanges();
                    }
                }
                
                json.Data = new { Code = "", Data = "", Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = "", Message = ex.Message };
            }
            return json;
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            GlsunView.Domain.User module = null;
            using (var ctx = new GlsunViewEntities())
            {
                module = (from u in ctx.User
                               where u.ID == id
                               select u).FirstOrDefault();
                ViewBag.RoleList = ctx.Role.ToList();
                var userRoleList = (from ur in ctx.v_UserRole
                                    where ur.UID == id
                                    select ur).ToList();
                ViewBag.UserRole = JsonHelper.getJsonByObject(userRoleList);
            }
            ViewBag.Action = "Edit";
            return View("Create", module);
        }

        // POST: User/Edit/5
        [HttpPost]
        public JsonResult Edit(User user, List<int> SelectedRoleId)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    var userModify = (from u in ctx.User
                              where u.ID == user.ID
                              select u).FirstOrDefault();
                    if(userModify.UPassword != user.UPassword)
                    {
                        _userLogger.RecordModify(loginUser, userModify, "修改密码", "", "成功", "");
                    }
                    //修改用户
                    userModify.ULoginName = user.ULoginName;
                    userModify.UPassword = user.UPassword;
                    userModify.UName = user.UName;
                    userModify.UUserType = user.UUserType;
                    userModify.UExpireTime = user.UExpireTime;
                    userModify.UIsLock = user.UIsLock;

                    userModify.EditorID = loginUser.ID;
                    userModify.EditingTime = DateTime.Now;
                    //修改用户角色
                    if (SelectedRoleId == null)
                        SelectedRoleId = new List<int>();
                    var userRole = (from ur in ctx.UserRole
                                    where ur.UID == userModify.ID
                                    select ur).ToList();
                    //删除未选中的
                    foreach(var ur in userRole)
                    {
                        if (!SelectedRoleId.Contains(ur.RID.Value))
                        {
                            //必须放在Remove方法之前，
                            var role = ctx.Role.Where(r => r.ID == ur.RID).FirstOrDefault();
                            ctx.UserRole.Remove(ur);
                            
                            if (role != null)
                            {
                                _userLogger.RecordModify(loginUser, userModify, "修改权限",
                                    string.Format("移除角色：{0}", role.RName), "成功", "");
                            }
                        }
                    }
                    //添加选中
                    var ownRoleId = (from ur in ctx.UserRole
                                     where ur.UID == userModify.ID
                                     select ur.RID).ToList();
                    foreach(var id in SelectedRoleId)
                    {
                        if (!ownRoleId.Contains(id))
                        {
                            var newUserRole = new UserRole
                            {
                                UID = userModify.ID,
                                RID = id,
                                CreatorID = loginUser.ID,
                                CreationTime = DateTime.Now
                            };
                            ctx.UserRole.Add(newUserRole);

                            var role = ctx.Role.Where( r=> r.ID == id).FirstOrDefault();
                            if (role != null)
                            {
                                _userLogger.RecordModify(loginUser, userModify, "修改权限",
                                    string.Format("分配角色：{0}", role.RName), "成功", "");
                            }
                        }
                    }
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = user, Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = user, Message = ex.Message };
            }
            return json;
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        public JsonResult Delete(int id, FormCollection collection)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var userDel = (from u in ctx.User
                                   where u.ID == id
                                   select u).FirstOrDefault();
                    var userRoleDel = from ur in ctx.UserRole
                                      where ur.UID == id
                                      select ur;
                    ctx.User.Remove(userDel);
                    ctx.UserRole.RemoveRange(userRoleDel);
                    ctx.SaveChanges();

                }
                json.Data = new { Code = "", Data = id, Message = "删除成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = id, Message = ex.Message };
            }
            return json;
        }
    }
}
