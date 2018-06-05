using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;
using System.Text;

namespace GlsunView.Controllers
{
    public class RoleController : ShareListController
    {
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int page = 1, int pageSize = 10)
        {
            IEnumerable<v_Role> roles = null;
            PagingInfo pagingInfo = null;
            using (var ctx = new GlsunViewEntities())
            {
                roles = ctx.v_Role.OrderBy(r => r.ID)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                pagingInfo = new PagingInfo
                {
                    TotalItems = ctx.v_Role.Count(),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    ShowPageCount = 5
                };
            }
            ViewBag.PagingInfo = pagingInfo;
            SetAuthorityData();
            return View(roles);
        }
        // GET: Role/Details/5
        public ActionResult Details(int id)
        {
            GlsunView.Domain.Role module = null;
            using (var ctx = new GlsunViewEntities())
            {
                module = (from r in ctx.Role
                          where r.ID == id
                          select r).FirstOrDefault();
            }
            ViewBag.Action = "Details";
            return View("Create", module);
        }

        // GET: Role/Create
        public ActionResult Create()
        {
            Role role = new Role();
            return View();
        }

        // POST: Role/Create
        [HttpPost]
        public ActionResult Create(Role role)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    if (loginUser != null)
                    {
                        role.CreatorID = loginUser.ID;
                        role.CreationTime = DateTime.Now;
                    }

                    ctx.Role.Add(role);
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = role, Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = role, Message = ex.Message };
            }
            return json;
        }

        // GET: Role/Edit/5
        public ActionResult Edit(int id)
        {
            GlsunView.Domain.Role module = null;
            using (var ctx = new GlsunViewEntities())
            {
                module = (from r in ctx.Role
                          where r.ID == id
                          select r).FirstOrDefault();
            }
            ViewBag.Action = "Edit";
            return View("Create", module);
        }

        // POST: Role/Edit/5
        [HttpPost]
        public ActionResult Edit(Role role)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();

                    var roleModify = (from r in ctx.Role
                                      where r.ID == role.ID
                                      select r).FirstOrDefault();

                    roleModify.RName = role.RName;
                    roleModify.RCode = role.RCode;
                    roleModify.EditorID = loginUser.ID;
                    roleModify.EditingTime = DateTime.Now;
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = role, Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = role, Message = ex.Message };
            }
            return json;
        }

        // GET: Role/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Role/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var roleDel = (from r in ctx.Role
                                   where r.ID == id
                                   select r).FirstOrDefault();
                    ctx.Role.Remove(roleDel);
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

        /// <summary>
        /// 角色授权
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        public ActionResult Authorize(int id)
        {
            //树形模块列表
            IEnumerable<v_ModuleTree> modules = null;
            //模块拥有权限
            IEnumerable<ModuleAuthItem> moduleAuths = null;
            //角色权限
            IEnumerable<RoleAuthority> roleAuths = null;
            using(var ctx = new GlsunViewEntities())
            {
                modules = ctx.v_ModuleTree.ToList().Where(m => m.MType == "menu");
                moduleAuths = (from m in ctx.Module
                              join ma in ctx.ModuleAuthority
                              on m.ID equals ma.MID
                              join a in ctx.Authority
                              on ma.AID equals a.ID
                              select new ModuleAuthItem
                              {
                                  MID = m.ID,
                                  MAID = ma.ID,
                                  AName = a.AName
                              }).ToList();
                roleAuths = (from r in ctx.RoleAuthority
                             where r.RID == id
                             select r).ToList();
            }
            List<AuthorityTreeNode> treeNodes = new List<AuthorityTreeNode>();
            List<int> roleOwnAuthIds = (from m in roleAuths
                                        select m.MAID.Value).ToList();
            
            foreach (var e in modules)
            {
                AuthorityTreeNode node = new AuthorityTreeNode
                {
                    ID = e.ID,  //模块表ID
                    PID = e.MParentID.Value,
                    Name = e.MName,
                    Checked = false,
                    DataID = e.ID
                };
                int nodeId = 1;
                //不是顶级菜单
                if (e.MParentID != 0 /*|| e.MName == "拓扑管理"*/)
                {
                    //遍历模块权限
                    foreach (var a in moduleAuths)
                    {
                        if (a.MID == e.ID)
                        {
                            AuthorityTreeNode nodeT = new AuthorityTreeNode
                            {
                                ID = e.ID + 10000 + nodeId++,    //模块权限ID
                                PID = e.ID,
                                Name = a.AName,
                                Checked = false,
                                DataID = a.MAID
                            };
                            //该角色拥有此模块权限则选中
                            if(roleOwnAuthIds != null && roleOwnAuthIds.Contains(a.MAID))
                            {
                                nodeT.Checked = true;
                                node.Checked = true;
                                //让父节点选择
                                foreach(var nd in treeNodes)
                                {
                                    if(nd.ID == node.PID)
                                    {
                                        nd.Checked = true;
                                    }
                                }
                            }
                            treeNodes.Add(nodeT);
                        }
                    }
                }
                treeNodes.Add(node);
            }

            StringBuilder sbText = new StringBuilder();
            sbText.Append("[");
            int cnt = 0;
            foreach(var e in treeNodes)
            {
                if(cnt == 0)
                {
                    sbText.Append(e.ToJSONObject());
                }
                else
                {
                    sbText.AppendFormat(",{0}", e.ToJSONObject());
                }
                cnt++;
            }
            sbText.Append("]");
            ViewBag.TreeNodes = sbText.ToString();
            return View(new RoleAuthrizeInfo { RoleId = id });
        }
        [HttpPost]
        public ActionResult Authorize(RoleAuthrizeInfo roleAuthInfo)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    //移除未勾选
                    if (roleAuthInfo.UnCheckedId != null)
                    {
                        foreach (var sId in roleAuthInfo.UnCheckedId)
                        {
                            int maId = Convert.ToInt32(sId);
                            var roleAuth = (from ra in ctx.RoleAuthority
                                            where ra.MAID == maId && ra.RID == roleAuthInfo.RoleId
                                            select ra).FirstOrDefault();
                            if (roleAuth != null)
                            {
                                ctx.RoleAuthority.Remove(roleAuth);
                            }
                        }
                    }
                    //添加勾选
                    if (roleAuthInfo.CheckedId != null)
                    {
                        foreach (var sId in roleAuthInfo.CheckedId)
                        {
                            int maId = Convert.ToInt32(sId);
                            var roleAuth = new RoleAuthority
                            {
                                RID = roleAuthInfo.RoleId,
                                MAID = maId,
                                CreatorID = loginUser.ID,
                                CreationTime = DateTime.Now
                            };
                            ctx.RoleAuthority.Add(roleAuth);
                        }
                    }
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = roleAuthInfo, Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = roleAuthInfo, Message = ex.Message };
            }
            return json;
        }
    }
}
