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
    public class RouteController : ShareListController
    {
        // GET: Route
        public ActionResult Index(int id)
        {
            Route route = null;
            using (var ctx = new GlsunViewEntities())
            {
                route = ctx.Route.Find(id);
            }
            return View(route);
        }
        /// <summary>
        /// 路由列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult List(int page = 1, int pageSize = 10)
        {
            IEnumerable<v_Route> routes = null;
            PagingInfo pagingInfo = null;
            using (var ctx = new GlsunViewEntities())
            {
                routes = ctx.v_Route.OrderBy(mr => mr.ID)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                //分页信息
                pagingInfo = new PagingInfo
                {
                    TotalItems = ctx.v_MachineRoom.Count(),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    ShowPageCount = 5
                };
            }
            ViewBag.PagingInfo = pagingInfo;
            SetAuthorityData();
            return View(routes);
        }
        // GET: Route/Details/5
        public ActionResult Details(int id)
        {
            Route route = null;
            using (var ctx = new GlsunViewEntities())
            {
                route = ctx.Route.Find(id);
            }
            ViewBag.Action = "Details";
            SetOptionData();
            return View("Create", route);
        }

        // GET: Route/Create
        public ActionResult Create()
        {
            Route route = new Route();
            SetOptionData();
            return View(route);
        }

        private void SetOptionData()
        {
            //路由组数据
            StringBuilder sbGroups = new StringBuilder();
            //机框数据
            StringBuilder sbFrames = new StringBuilder();
            sbGroups.Append("[");
            sbFrames.Append("[");
            using (var ctx = new GlsunViewEntities())
            {
                int count = 0;
                foreach (var group in ctx.RouteGroup)
                {
                    sbGroups.AppendFormat("{{id: {0}, text: '{1}'}}", group.ID, group.RGName);
                    count++;
                    if (count != ctx.RouteGroup.Count())
                    {
                        sbGroups.Append(",");
                    }
                }
                count = 0;
                foreach (var frame in ctx.MachineFrame)
                {
                    sbFrames.AppendFormat("{{id: {0}, text: '{1}'}}", frame.ID, frame.MFName);
                    count++;
                    if (count != ctx.MachineFrame.Count())
                    {
                        sbFrames.Append(",");
                    }
                }
            }
            sbGroups.Append("]");
            sbFrames.Append("]");
            ViewBag.GroupData = sbGroups.ToString();
            ViewBag.FrameData = sbFrames.ToString();
        }

        // POST: Route/Create
        [HttpPost]
        public ActionResult Create(Route route)
        {
            var json = new JsonResult();
            User loginUser = null;
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                 where u.ULoginName == HttpContext.User.Identity.Name
                                 select u).FirstOrDefault();
                    var frameA = ctx.MachineFrame.Find(route.RAMFID);
                    var frameB = ctx.MachineFrame.Find(route.RBMFID);
                    route.RAIP = frameA.MFIP;
                    route.RBIP = frameB.MFIP;
                    route.CreationTime = DateTime.Now;
                    route.CreatorID = loginUser.ID;

                    ctx.Route.Add(route);
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = "", Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = "", Message = ex.Message };
            }
            return json;
        }

        // GET: Route/Edit/5
        public ActionResult Edit(int id)
        {
            Route route = null;
            using (var ctx = new GlsunViewEntities())
            {
                route = ctx.Route.Find(id);
            }
            ViewBag.Action = "Edit";
            SetOptionData();
            return View("Create", route);
        }

        // POST: Route/Edit/5
        [HttpPost]
        public ActionResult Edit(Route route)
        {
            var json = new JsonResult();
            User loginUser = null;
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                 where u.ULoginName == HttpContext.User.Identity.Name
                                 select u).FirstOrDefault();
                    var modifyRoute = ctx.Route.Find(route.ID);
                    modifyRoute.RName = route.RName;
                    modifyRoute.RType = route.RType;
                    modifyRoute.RGID = route.RGID;
                    modifyRoute.RAName = route.RAName;
                    modifyRoute.RAMFID = route.RAMFID;
                    modifyRoute.RAIP = route.RAIP;
                    modifyRoute.RASlot = route.RASlot;
                    modifyRoute.RBName = route.RBName;
                    modifyRoute.RBMFID = route.RBMFID;
                    modifyRoute.RBIP = route.RBIP;
                    modifyRoute.RBSlot = route.RBSlot;
                    modifyRoute.Remark = route.Remark;
                    modifyRoute.EditorID = loginUser.ID;
                    modifyRoute.EditingTime = DateTime.Now;
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = "", Message = "保存成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = "", Message = ex.Message };
            }
            return json;
        }

        // GET: Route/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Route/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    ctx.Route.Remove(ctx.Route.Find(id));
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = "", Message = "删除成功" };
            }
            catch (Exception ex)
            {
                json.Data = new { Code = "Exception", Data = "", Message = ex.Message };
            }
            return json;
        }
    }
}
