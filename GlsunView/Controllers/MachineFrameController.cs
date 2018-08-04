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
    public class MachineFrameController : ShareListController
    {
        // GET: MachineFrame
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(int page = 1, int pageSize = 10)
        {
            IEnumerable<v_MachineFrame> frames = null;
            PagingInfo pagingInfo = null;
            using (var ctx = new GlsunViewEntities())
            {
                frames = ctx.v_MachineFrame.OrderBy(ms => ms.ID)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                pagingInfo = new PagingInfo
                {
                    TotalItems = ctx.v_MachineFrame.Count(),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    ShowPageCount = 5
                };
            }
            ViewBag.PagingInfo = pagingInfo;
            SetAuthorityData();
            return View(frames);
        }
        // GET: MachineFrame/Details/5
        public ActionResult Details(int id)
        {
            MachineFrame frame = null;
            int roomID = 0;
            using (var ctx = new GlsunViewEntities())
            {
                frame = ctx.MachineFrame.Find(id);
                if (frame != null)
                {
                    roomID = (from shelf in ctx.MachineShelf
                              join room in ctx.MachineRoom
                              on shelf.MRID equals room.ID
                              where shelf.ID == frame.MSID.Value
                              select room.ID).First();
                }
            }
            SetMachineData();
            ViewBag.Action = "Details";
            ViewBag.RoomID = roomID;
            return View("Create", frame);
        }

        // GET: MachineFrame/Create
        public ActionResult Create()
        {
            MachineFrame frame = new MachineFrame();
            SetMachineData();
            ViewBag.Action = "Create";
            return View(frame);
        }

        private void SetMachineData()
        {
            //机房数据
            StringBuilder sbRooms = new StringBuilder();
            //机房对应机架数据
            StringBuilder sbRoomShelfs = new StringBuilder();
            //机架层数数据
            StringBuilder sbShelfLayers = new StringBuilder();
            sbRooms.Append("[");
            sbRoomShelfs.Append("{");
            sbShelfLayers.Append("{");
            using (var ctx = new GlsunViewEntities())
            {
                int count = 0;
                foreach (var room in ctx.MachineRoom)
                {
                    sbRooms.AppendFormat("{{id: {0}, text: '{1}'}}", room.ID, room.MRName);
                    count++;

                    sbRoomShelfs.AppendFormat("{0}:[", room.ID);
                    var shelfs = ctx.MachineShelf.Where(s => s.MRID == room.ID);
                    int cnt = 0;
                    foreach (var shelf in shelfs)
                    {
                        sbRoomShelfs.AppendFormat("{{id: {0}, text: '{1}'}}", shelf.ID, shelf.MSName);
                        cnt++;
                        if (cnt != shelfs.Count())
                        {
                            sbRoomShelfs.Append(",");
                        }
                    }
                    sbRoomShelfs.Append("]");

                    if (count != ctx.MachineRoom.Count())
                    {
                        sbRooms.Append(",");
                        sbRoomShelfs.Append(",");
                    }
                }
                count = 0;
                foreach (var shelf in ctx.MachineShelf)
                {
                    sbShelfLayers.AppendFormat("{0}:{1}", shelf.ID, shelf.MSLayers);
                    count++;
                    if (count != ctx.MachineShelf.Count())
                    {
                        sbShelfLayers.Append(",");
                    }
                }
            }
            sbRooms.Append("]");
            sbRoomShelfs.Append("}");
            sbShelfLayers.Append("}");
            ViewBag.RoomData = sbRooms.ToString();
            ViewBag.RoomShelfData = sbRoomShelfs.ToString();
            ViewBag.ShelfLayerData = sbShelfLayers.ToString();
        }

        // POST: MachineFrame/Create
        [HttpPost]
        public ActionResult Create(MachineFrame frame)
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
                        frame.CreatorID = loginUser.ID;
                        frame.CreationTime = DateTime.Now;
                    }

                    ctx.MachineFrame.Add(frame);
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

        // GET: MachineFrame/Edit/5
        public ActionResult Edit(int id)
        {
            MachineFrame frame = null;
            int roomID = 0;
            using (var ctx = new GlsunViewEntities())
            {
                frame = ctx.MachineFrame.Find(id);
                if(frame != null)
                {
                    roomID = (from shelf in ctx.MachineShelf
                             join room in ctx.MachineRoom
                             on shelf.MRID equals room.ID
                             where shelf.ID == frame.MSID.Value
                             select room.ID).First();
                }
            }
            SetMachineData();
            ViewBag.Action = "Edit";
            ViewBag.RoomID = roomID;
            return View("Create", frame);
        }

        // POST: MachineFrame/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MachineFrame/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MachineFrame/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
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
    }
}
