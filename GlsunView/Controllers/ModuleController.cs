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
    public class ModuleController : ShareListController
    {
        // GET: Module
        public ActionResult Index()
        {
            IEnumerable<v_ModuleTree> modules = null;
            using(var ctx = new GlsunViewEntities())
            {
                modules = ctx.v_ModuleTree.ToList();
            }
            SetAuthorityData();
            return View(modules);
        }
        
        // GET: Module/Details/5
        public ActionResult Details(int id)
        {
            Module module = null;
            using (var ctx = new GlsunViewEntities())
            {
                ViewBag.Modules = ctx.Module.ToList();
                module = (from m in ctx.Module
                          where m.ID == id
                          select m).FirstOrDefault();
            }
            ViewBag.ReadOnly = true;
            ViewBag.Action = "Details";
            return View("Create", module);
        }

        // GET: Module/Create
        public ActionResult Create()
        {
            Module module = null;
            using (var ctx = new GlsunViewEntities())
            {
                ViewBag.Modules = ctx.Module.ToList();
                module = ctx.Module.Create();
            }
            module.MLevel = 1;
            module.MUrl = "#";
            module.MIconType = "font";
            module.MSortingNumber = 0;
            module.IsEnabled = true;
            module.MParentID = 0;
            ViewBag.Action = "Create";
            return View(module);
        }

        // POST: Module/Create
        [HttpPost]
        public JsonResult Create(Module module)
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
                        if (loginUser != null)
                        {
                            module.CreatorID = loginUser.ID;
                            module.CreationTime = DateTime.Now;
                        }

                        ctx.Module.Add(module);
                        //添加操作权限
                        if (module.MParentID != 0 && module.MType == "menu")
                        {
                            foreach (var e in ctx.Authority.Where(a => a.IsEnabled.Value && a.AIsCommon.Value))
                            {
                                ModuleAuthority ma = new ModuleAuthority
                                {
                                    AID = e.ID,
                                    MID = module.ID,
                                    CreatorID = loginUser.ID,
                                    CreationTime = DateTime.Now
                                };
                                ctx.ModuleAuthority.Add(ma);
                            }
                        }
                        ctx.SaveChanges();
                        tran.Complete();  //必须调用.Complete()，不然数据不会保存
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

        // GET: Module/Edit/5
        public ActionResult Edit(int id)
        {
            Module module = null;
            using (var ctx = new GlsunViewEntities())
            {
                ViewBag.Modules = ctx.Module.ToList();
                module = (from m in ctx.Module
                          where m.ID == id
                          select m).FirstOrDefault();
            }
            ViewBag.Action = "Edit";
            return View("Create",module);
        }

        // POST: Module/Edit/5
        [HttpPost]
        public JsonResult Edit(Module module)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    var oldModule = (from m in ctx.Module
                                     where m.ID == module.ID
                                     select m).FirstOrDefault();

                    oldModule.MName = module.MName;
                    oldModule.MUrl = module.MUrl;
                    oldModule.MParentID = module.MParentID;
                    oldModule.MLevel = module.MLevel;
                    oldModule.MIconType = module.MIconType;
                    oldModule.MIcon = module.MIcon;
                    oldModule.MSortingNumber = module.MSortingNumber;
                    oldModule.MType = module.MType;
                    oldModule.IsEnabled = module.IsEnabled;
                    oldModule.EditorID = loginUser.ID;
                    oldModule.EditingTime = DateTime.Now;

                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = module, Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = module, Message = ex.Message };
            }
            return json;
        }

        // GET: Module/Delete/5
        public ActionResult Delete(int id)
        {    
            return View();
        }

        // POST: Module/Delete/5
        [HttpPost]
        public JsonResult Delete(int id, FormCollection collection)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var module = (from m in ctx.Module
                                  where m.ID == id
                                  select m).First();
                    DeleteChildModule(module.ID);
                    var moduleAuthority = from ma in ctx.ModuleAuthority
                                          where ma.MID == id
                                          select ma;
                    ctx.ModuleAuthority.RemoveRange(moduleAuthority);
                    ctx.Module.Remove(module);
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

        public void DeleteChildModule(int pId)
        {
            using (var ctx = new GlsunViewEntities())
            {
                var childModule = from m in ctx.Module
                             where m.MParentID == pId
                             select m;
                if(childModule.Count() > 0)
                {
                    foreach(var m in childModule)
                    {
                        var moduleAuthority = from ma in ctx.ModuleAuthority
                                              where ma.MID == m.ID
                                              select ma;
                        ctx.ModuleAuthority.RemoveRange(moduleAuthority);

                        DeleteChildModule(m.ID);
                    }

                    ctx.Module.RemoveRange(childModule);
                    ctx.SaveChanges();
                }
            }
        }

        public ActionResult Authority(int id)
        {
            ModuleAuth module = new ModuleAuth();
            module.ModuleID = id;
            using(var ctx = new GlsunViewEntities())
            {
                var theModule = ctx.Module.Find(id);
                if(theModule != null && (theModule.MParentID != 0 || theModule.MName == "拓扑管理"))
                {
                    module.Authorities = ctx.Authority.ToList();
                    module.OwnAuthorityID = (from m in ctx.Module
                                             join ma in ctx.ModuleAuthority on m.ID equals ma.MID
                                             join a in ctx.Authority on ma.AID equals a.ID
                                             where m.ID == id
                                             select a.ID).ToList();
                }
                else
                {
                    module.Authorities = new List<Authority>();
                    module.OwnAuthorityID = new List<int>();
                }
            }
            return View(module);
        }
        [HttpPost]
        public ActionResult Authority(ModuleAuth moduleAuth)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    //删除未选中的
                    IEnumerable<Authority> unselected = null;
                    if (moduleAuth.OwnAuthorityID == null)
                    {
                        unselected = ctx.Authority.ToList();
                    }
                    else
                    {
                        unselected = ctx.Authority.Where(a => !moduleAuth.OwnAuthorityID.Contains(a.ID));
                    }
                    foreach(var e in unselected)
                    {
                        var maDelete = (from ma in ctx.ModuleAuthority
                                        where ma.AID == e.ID && ma.MID == moduleAuth.ModuleID
                                        select ma).FirstOrDefault();
                        if(maDelete != null)
                            ctx.ModuleAuthority.Remove(maDelete);
                    }
                    //添加选中的
                    if (moduleAuth.OwnAuthorityID != null)
                    {
                        foreach (var id in moduleAuth.OwnAuthorityID)
                        {
                            var maAdd = (from ma in ctx.ModuleAuthority
                                         where ma.AID == id && ma.MID == moduleAuth.ModuleID
                                         select ma).FirstOrDefault();
                            //不存在则添加
                            if (maAdd == null)
                            {
                                ModuleAuthority ma = new ModuleAuthority
                                {
                                    MID = moduleAuth.ModuleID,
                                    AID = id,
                                    CreatorID = loginUser.ID,
                                    CreationTime = DateTime.Now
                                };
                                ctx.ModuleAuthority.Add(ma);
                            }
                        } 
                    }
                    ctx.SaveChanges();                                       
                }
                json.Data = new { Code = "", Data = moduleAuth, Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = moduleAuth, Message = ex.Message };
            }
            return json;
        }
    }
}
