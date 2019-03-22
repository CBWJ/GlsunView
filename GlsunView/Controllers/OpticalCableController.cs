using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;
using Newtonsoft.Json;

namespace GlsunView.Controllers
{
    public class OpticalCableController : ShareListController
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
        // GET: OpticalCable
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(int page = 1, int pageSize = 10, string keyword = "")
        {
            IEnumerable<OpticalCable> cables = db.OpticalCable.ToList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                cables = cables.Where(m => m.OC_Name.ToUpper().Contains(keyword.ToUpper())).ToList();
            }
            var count = cables.Count();
            cables = cables.OrderBy(m => m.ID)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

            PagingInfo pagingInfo = new PagingInfo
            {
                TotalItems = count,
                CurrentPage = page,
                ItemsPerPage = pageSize,
                ShowPageCount = 5
            };
            ViewBag.PagingInfo = pagingInfo;
            ViewData["keyword"] = keyword;
            SetAuthorityData();
            return View(cables);
        }

        public ActionResult Create(int idA = 0, int idB = 0, string nodes = "", decimal distance = 0)
        {
            OpticalCable cable = new OpticalCable();
            cable.OC_MarkIDA = idA;
            cable.OC_MarkIDB = idB;
            cable.OC_Nodes = nodes;
            cable.OC_Length = distance;
            cable.OC_Name = string.Format("{0}—{1}", db.Mark.Find(idA).MName, db.Mark.Find(idB).MName);
            CreateAction();
            ViewBag.Marks = db.Mark.ToList();
            return View(cable);
        }
        [HttpPost]
        public ActionResult Create(OpticalCable cable)
        {
            JsonResult ret = new JsonResult();
            try
            {
                AddModelToContext(cable);
                for(int i = 0; i < cable.OC_FiberCount; ++i)
                {
                    OpticalFiber fiber = new OpticalFiber
                    {
                        OF_Name = string.Format("#{0}", i + 1),
                        OC_ID = cable.ID,
                        OC_Name = cable.OC_Name,
                        OC_TotalLength = cable.OC_TotalLength
                    };
                    AddModelToContext(fiber);
                }
                db.SaveChanges();
                ret.Data = JsonConvert.SerializeObject(new
                {
                    status = 0,
                    message = "",
                    data = cable
                });
            }
            catch(Exception ex)
            {
                ret.Data = JsonConvert.SerializeObject(new
                {
                    status = 1,
                    message = ex.Message,
                    data = ""
                });
            }
            return ret;
        }

        public ActionResult Edit(int id)
        {
            var cable = db.OpticalCable.Find(id);
            EditAction();
            ViewBag.Marks = db.Mark.ToList();
            return View("Create", cable);
        }
        [HttpPost]
        public ActionResult Edit(OpticalCable cable)
        {
            JsonResult ret = new JsonResult();
            try
            {
                EditModelToContext(cable);
                //删除旧的
                var fibers = db.OpticalFiber.Where(of => of.OC_ID == cable.ID);
                db.OpticalFiber.RemoveRange(fibers.ToArray());
                for (int i = 0; i < cable.OC_FiberCount; ++i)
                {
                    OpticalFiber fiber = new OpticalFiber
                    {
                        OF_Name = string.Format("#{0}", i + 1),
                        OC_ID = cable.ID,
                        OC_Name = cable.OC_Name,
                        OC_TotalLength = cable.OC_TotalLength
                    };
                    AddModelToContext(fiber);
                }
                db.SaveChanges();
                ret.Data = JsonConvert.SerializeObject(new
                {
                    status = 0,
                    message = "",
                    data = cable
                });
            }
            catch (Exception ex)
            {
                ret.Data = JsonConvert.SerializeObject(new
                {
                    status = 1,
                    message = ex.Message,
                    data = cable
                });
            }
            return ret;
        }

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            JsonResult ret = new JsonResult();
            try
            {
                foreach(var id in ids)
                {
                    var cable = db.OpticalCable.Find(id);
                    if (cable != null)
                    {
                        //新删除依赖项
                        var fibers = db.OpticalFiber.Where(of => of.OC_ID == cable.ID);
                        db.OpticalFiber.RemoveRange(fibers.ToArray());
                        //再删除数据
                        DeleteModelFromContext(cable);
                    }
                }
                
                db.SaveChanges();
                ret.Data = JsonConvert.SerializeObject(new
                {
                    status = 0,
                    message = ""
                });
            }
            catch (Exception ex)
            {
                ret.Data = JsonConvert.SerializeObject(new
                {
                    status = 1,
                    message = ex.Message
                });
            }
            return ret;
        }
        public ActionResult Details(int id)
        {
            DetailsAction();
            var cable = db.OpticalCable.Find(id);
            ViewBag.Marks = db.Mark.ToList();
            return View("Create", cable);
        }
    }
}