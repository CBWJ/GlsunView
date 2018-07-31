using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;

namespace GlsunView.Controllers
{
    public class MachineRoomController : ShareListController
    {
        // GET: MachineRoom
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int page = 1, int pageSize = 10)
        {
            IEnumerable<v_MachineRoom> rooms = null;
            PagingInfo pagingInfo = null;
            using (var ctx = new GlsunViewEntities())
            {
                rooms = ctx.v_MachineRoom.OrderBy(mr => mr.ID)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
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
            return View(rooms);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            MachineRoom room = new MachineRoom();
            return View(room);
        }
        [HttpPost]
        public ActionResult Create(MachineRoom room)
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
                        room.CreatorID = loginUser.ID;
                        room.CreationTime = DateTime.Now;
                    }

                    ctx.MachineRoom.Add(room);
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
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            MachineRoom room = null;
            using (var ctx = new GlsunViewEntities())
            {
                room = ctx.MachineRoom.Find(id);
            }
            ViewBag.Action = "Edit";
            return View("Create", room);
        }
        [HttpPost]
        public ActionResult Edit(MachineRoom room)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();

                    var roomModify = (from r in ctx.MachineRoom
                                      where r.ID == room.ID
                                      select r).FirstOrDefault();

                    roomModify.MRName = room.MRName;
                    roomModify.MRAddress = room.MRAddress;
                    roomModify.MRDescription = room.MRDescription;
                    roomModify.MRIcon = room.MRIcon;
                    roomModify.Remark = room.Remark;
                    roomModify.EditorID = loginUser.ID;
                    roomModify.EditingTime = DateTime.Now;
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
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var roomDelete = ctx.MachineRoom.Find(id);
                    ctx.MachineRoom.Remove(roomDelete);
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
        /// 查看
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            MachineRoom room = null;
            using (var ctx = new GlsunViewEntities())
            {
                room = ctx.MachineRoom.Find(id);
            }
            ViewBag.Action = "Details";
            return View("Create", room);
        }
    }
}