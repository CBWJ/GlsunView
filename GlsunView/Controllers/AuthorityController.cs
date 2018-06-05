using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;
using System.Transactions;

namespace GlsunView.Controllers
{
    public class AuthorityController : ShareListController
    {
        // GET: Authority
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int page = 1, int pageSize = 10)
        {
            IEnumerable<v_Authority> authorities = null;
            int itemCount = 0;
            using (var ctx = new GlsunViewEntities())
            {
                itemCount = ctx.v_Authority.Count();
                authorities = ctx.v_Authority.OrderBy(a => a.ID)
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
            return View(authorities);
        }
        // GET: Authority/Details/5
        public ActionResult Details(int id)
        {
            Authority auth = null;
            using (var ctx = new GlsunViewEntities())
            {
                auth = (from a in ctx.Authority
                        where a.ID == id
                        select a).FirstOrDefault();
            }
            ViewBag.Action = "Details";
            return View("Create", auth);
        }

        // GET: Authority/Create
        public ActionResult Create()
        {
            Authority auth = new Authority
            {
                AType = "button",
                AIconType = "font",
                AIsCommon = true,
                IsEnabled = true
            };
            ViewBag.Action = "Create";
            return View(auth);
        }

        // POST: Authority/Create
        [HttpPost]
        public ActionResult Create(Authority authority)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    using (var tran = new TransactionScope())//开启事务
                    {
                        var loginUser = (from u in ctx.User
                                         where u.ULoginName == HttpContext.User.Identity.Name
                                         select u).FirstOrDefault();

                        authority.CreatorID = loginUser.ID;
                        authority.CreationTime = DateTime.Now;
                        ctx.Authority.Add(authority);

                        //给模块添加新权限--通用权限才添加

                        if (authority.AIsCommon == true)
                        {
                            var moduleList = ctx.Module.Where(m => m.MParentID != 0 && m.MType == "menu");
                            foreach (var module in moduleList)
                            {
                                ModuleAuthority ma = new ModuleAuthority
                                {
                                    AID = authority.ID,
                                    MID = module.ID,
                                    CreatorID = loginUser.ID,
                                    CreationTime = DateTime.Now
                                };
                                ctx.ModuleAuthority.Add(ma);
                            } 
                        }
                        ctx.SaveChanges();

                        tran.Complete();//必须调用.Complete()，不然数据不会保存
                    }//出了using代码块如果还没调用Complete()，所有操作就会自动回滚
                }

                json.Data = new { Code = "", Data = "", Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = "", Message = ex.Message };
            }
            return json;
        }

        // GET: Authority/Edit/5
        public ActionResult Edit(int id)
        {
            Authority auth = null;
            using (var ctx = new GlsunViewEntities())
            {
                auth = (from a in ctx.Authority
                        where a.ID == id
                        select a).FirstOrDefault();
            }
            ViewBag.Action = "Edit";
            return View("Create", auth);
        }

        // POST: Authority/Edit/5
        [HttpPost]
        public ActionResult Edit(Authority authority)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    using (var tran = new TransactionScope())
                    {
                        var loginUser = (from u in ctx.User
                                         where u.ULoginName == HttpContext.User.Identity.Name
                                         select u).FirstOrDefault();

                        var authModify = ctx.Authority.Find(authority.ID);
                        if (authModify != null)
                        {
                            var moduleList = ctx.Module.Where(m => m.MParentID != 0 && m.MType == "menu");
                            //通用选项修改
                            if (authority.AIsCommon != authModify.AIsCommon)
                            {
                                //改为通用，添加
                                if (authority.AIsCommon == true)
                                {
                                    foreach (var module in moduleList)
                                    {
                                        ModuleAuthority ma = new ModuleAuthority
                                        {
                                            AID = authority.ID,
                                            MID = module.ID,
                                            CreatorID = loginUser.ID,
                                            CreationTime = DateTime.Now
                                        };
                                        ctx.ModuleAuthority.Add(ma);
                                    }
                                }
                                else//否则删除
                                {
                                    foreach (var module in moduleList)
                                    {
                                        var maDelete = (from ma in ctx.ModuleAuthority
                                                        where ma.AID == authority.ID && ma.MID == module.ID
                                                        select ma).FirstOrDefault();
                                        if (maDelete != null)
                                            ctx.ModuleAuthority.Remove(maDelete);
                                    }
                                }
                            }
                            authModify.AName = authority.AName;
                            authModify.ACode = authority.ACode;
                            authModify.AType = authority.AType;
                            authModify.AIcon = authority.AIcon;
                            authModify.AIconType = authority.AIconType;
                            authModify.AClassName = authority.AClassName;
                            authModify.AShowNumber = authority.AShowNumber;
                            authModify.AIsCommon = authority.AIsCommon;
                            authModify.IsEnabled = authority.IsEnabled;

                            authority.EditorID = loginUser.ID;
                            authority.EditingTime = DateTime.Now;
                        }
                        ctx.SaveChanges();
                        tran.Complete();
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

        // GET: Authority/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Authority/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    using (var tran = new TransactionScope())
                    {
                        var authDelete = ctx.Authority.Find(id);

                        var moduleAuth = ctx.ModuleAuthority.Where(m => m.AID == id);
                        //删除外键引用记录
                        foreach (var ma in moduleAuth)
                        {
                            var roleAuth = ctx.RoleAuthority.Where(r => r.MAID == ma.ID);
                            ctx.RoleAuthority.RemoveRange(roleAuth);
                            ctx.ModuleAuthority.Remove(ma);
                        }
                        ctx.Authority.Remove(authDelete);

                        ctx.SaveChanges();
                        tran.Complete();
                    }
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
