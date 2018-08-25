using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;
using System.IO;

namespace GlsunView.Controllers
{
    public class RouteGroupController : ShareListController
    {
        // GET: RouteGroup
        public ActionResult Index(int id)
        {
            List<Route> routes = null;
            using (var ctx = new GlsunViewEntities())
            {
                routes = ctx.Route.Where(r => r.RGID == id).ToList();
            }
            return View(routes);
        }
        /// <summary>
        /// 路由组列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult List(int page = 1, int pageSize = 10)
        {
            IEnumerable<v_RouteGroup> groups = null;
            PagingInfo pagingInfo = null;
            using (var ctx = new GlsunViewEntities())
            {
                groups = ctx.v_RouteGroup.OrderBy(mr => mr.ID)
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
            return View(groups);
        }
        // GET: RouteGroup/Details/5
        public ActionResult Details(int id)
        {
            RouteGroup group = null;
            using (var ctx = new GlsunViewEntities())
            {
                group = ctx.RouteGroup.Find(id);
            }
            ViewBag.Action = "Details";
            return View("Create", group);
        }

        // GET: RouteGroup/Create
        public ActionResult Create()
        {
            RouteGroup group = new RouteGroup { RGIcon = "group.png" };
            return View(group);
        }

        // POST: RouteGroup/Create
        [HttpPost]
        public ActionResult Create(RouteGroup group, HttpPostedFileBase iconFile)
        {
            var json = new JsonResult();
            User loginUser = null;
            try
            {
                string iconFileName = "";
                if (iconFile != null)
                {
                    iconFileName = SaveIcon(iconFile);
                }
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                 where u.ULoginName == HttpContext.User.Identity.Name
                                 select u).FirstOrDefault();
                    group.CreatorID = loginUser.ID;
                    group.CreationTime = DateTime.Now;
                    //图标没值
                    if (string.IsNullOrWhiteSpace(group.RGIcon))
                    {
                        group.RGIcon = iconFileName;
                    }
                    ctx.RouteGroup.Add(group);
                    ctx.SaveChanges();
                }
                json.Data = new { Code = "", Data = "", Message = "保存成功" };
            }
            catch(Exception ex)
            {
                json.Data = new { Code = "Exception", Data = "", Message = ex.Message };
            }
            return json;
        }
        /// <summary>
        /// 保存图标
        /// </summary>
        /// <param name="iconFile"></param>
        /// <returns></returns>
        private string SaveIcon(HttpPostedFileBase iconFile)
        {
            var iconPath = Server.MapPath("~/image/route");
            DirectoryInfo directoryInfo = new DirectoryInfo(iconPath);
            var imgFiles = directoryInfo.GetFiles();
            var subnetFiles = (from f in imgFiles
                               where f.Name.ToLower().Contains("group")
                               select f).ToList();
            var fileType = iconFile.FileName.Substring(iconFile.FileName.IndexOf(".") + 1);
            var fileName = "group" + (subnetFiles.Count + 100).ToString() + "." + fileType;
            var filePath = Path.Combine(iconPath, fileName);
            byte[] byteIcon = new byte[iconFile.ContentLength];
            iconFile.InputStream.Read(byteIcon, 0, byteIcon.Length);
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                fs.Write(byteIcon, 0, byteIcon.Length);
            }
            return fileName;
        }
        // GET: RouteGroup/Edit/5
        public ActionResult Edit(int id)
        {
            RouteGroup group = null;
            using(var ctx = new GlsunViewEntities())
            {
                group = ctx.RouteGroup.Find(id);
            }
            ViewBag.Action = "Edit";
            return View("Create", group);
        }

        // POST: RouteGroup/Edit/5
        [HttpPost]
        public ActionResult Edit(RouteGroup group, HttpPostedFileBase iconFile)
        {
            var json = new JsonResult();
            User loginUser = null;
            try
            {
                string iconFileName = "";
                if (iconFile != null)
                {
                    iconFileName = SaveIcon(iconFile);
                }
                using (var ctx = new GlsunViewEntities())
                {
                    loginUser = (from u in ctx.User
                                 where u.ULoginName == HttpContext.User.Identity.Name
                                 select u).FirstOrDefault();
                    var modifyGroup = ctx.RouteGroup.Find(group.ID);
                    modifyGroup.RGName = group.RGName;
                    modifyGroup.RGIcon = group.RGIcon;
                    modifyGroup.Remark = group.Remark;
                    modifyGroup.EditorID = loginUser.ID;
                    modifyGroup.EditingTime = DateTime.Now;
                    //图标没值
                    if (string.IsNullOrWhiteSpace(modifyGroup.RGIcon))
                    {
                        modifyGroup.RGIcon = iconFileName;
                    }
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

        // GET: RouteGroup/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RouteGroup/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    ctx.RouteGroup.Remove(ctx.RouteGroup.Find(id));
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
        /// <summary>
        /// 拓扑图标选择
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult IconList(string type = "group")
        {
            var iconPath = Server.MapPath("~/image/route");
            DirectoryInfo directoryInfo = new DirectoryInfo(iconPath);
            var imgFiles = directoryInfo.GetFiles();
            var groupFiles = (from f in imgFiles
                               where f.Name.ToLower().Contains(type)
                               select f).ToList();
            ViewBag.IconPath = "/image/route/";
            return View("~/Views/Subnet/IconList.cshtml", groupFiles);
        }
    }
}
