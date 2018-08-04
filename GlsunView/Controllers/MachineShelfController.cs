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
    public class MachineShelfController : ShareListController
    {
        // GET: MachineShelf
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(int page = 1, int pageSize = 10)
        {
            IEnumerable<v_MachineShelf> shelfs = null;
            PagingInfo pagingInfo = null;
            using (var ctx = new GlsunViewEntities())
            {
                shelfs = ctx.v_MachineShelf.OrderBy(ms => ms.ID)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                pagingInfo = new PagingInfo
                {
                    TotalItems = ctx.v_MachineShelf.Count(),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    ShowPageCount = 5
                };
            }
            ViewBag.PagingInfo = pagingInfo;
            SetAuthorityData();
            return View(shelfs);
        }
        // GET: MachineShelf/Details/5
        public ActionResult Details(int id)
        {
            MachineShelf shelf = null;
            StringBuilder sbRooms = new StringBuilder();
            sbRooms.Append("[");
            using (var ctx = new GlsunViewEntities())
            {
                shelf = ctx.MachineShelf.Find(id);
                int count = 0;
                foreach (var room in ctx.MachineRoom)
                {
                    sbRooms.AppendFormat("{{id: {0}, text: '{1}'}}", room.ID, room.MRName);
                    count++;
                    if (count != ctx.MachineRoom.Count())
                        sbRooms.Append(",");
                }
            }
            sbRooms.Append("]");
            ViewBag.RoomData = sbRooms.ToString();
            ViewBag.Action = "Details";
            return View("Create", shelf);
        }

        // GET: MachineShelf/Create
        public ActionResult Create()
        {
            MachineShelf shelf = new MachineShelf();
            StringBuilder sbRooms = new StringBuilder();
            sbRooms.Append("[");
            using (var ctx = new GlsunViewEntities())
            {
                int count = 0;
                foreach(var room in ctx.MachineRoom)
                {
                    sbRooms.AppendFormat("{{id: {0}, text: '{1}'}}", room.ID, room.MRName);
                    count++;
                    if (count != ctx.MachineRoom.Count())
                        sbRooms.Append(",");
                }
            }
            sbRooms.Append("]");
            ViewBag.RoomData = sbRooms.ToString();
            ViewBag.Action = "Create";
            return View(shelf);
        }

        // POST: MachineShelf/Create
        [HttpPost]
        public ActionResult Create(MachineShelf shelf)
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
                        shelf.CreatorID = loginUser.ID;
                        shelf.CreationTime = DateTime.Now;
                    }

                    ctx.MachineShelf.Add(shelf);
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

        // GET: MachineShelf/Edit/5
        public ActionResult Edit(int id)
        {
            MachineShelf shelf = null;
            StringBuilder sbRooms = new StringBuilder();
            sbRooms.Append("[");
            using (var ctx = new GlsunViewEntities())
            {
                shelf = ctx.MachineShelf.Find(id);
                int count = 0;
                foreach (var room in ctx.MachineRoom)
                {
                    sbRooms.AppendFormat("{{id: {0}, text: '{1}'}}", room.ID, room.MRName);
                    count++;
                    if (count != ctx.MachineRoom.Count())
                        sbRooms.Append(",");
                }
            }
            sbRooms.Append("]");
            ViewBag.RoomData = sbRooms.ToString();
            ViewBag.Action = "Edit";
            return View("Create", shelf);
        }

        // POST: MachineShelf/Edit/5
        [HttpPost]
        public ActionResult Edit(MachineShelf shelf)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();

                    var shelfModify = (from r in ctx.MachineShelf
                                       where r.ID == shelf.ID
                                      select r).FirstOrDefault();

                    shelfModify.MSName = shelf.MSName;
                    shelfModify.MSNumber = shelf.MSNumber;
                    shelfModify.MSLayers = shelf.MSLayers;
                    shelfModify.MSIcon = shelf.MSIcon;
                    shelfModify.MRID = shelf.MRID;
                    shelfModify.Remark = shelf.Remark;
                    shelfModify.EditorID = loginUser.ID;
                    shelfModify.EditingTime = DateTime.Now;

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

        // GET: MachineShelf/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: MachineShelf/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var json = new JsonResult();
            try
            {
                using (var ctx = new GlsunViewEntities())
                {
                    var shelfDelete = ctx.MachineShelf.Find(id);
                    ctx.MachineShelf.Remove(shelfDelete);
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
