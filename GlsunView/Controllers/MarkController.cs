using GlsunView.Domain;
using GlsunView.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlsunView.Controllers
{
    public class MarkController : ShareListController
    {
        // GET: Mark
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(int page = 1, int pageSize = 10, string keyword="")
        {
            IEnumerable<Mark> marks = db.Mark.ToList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                marks = marks.Where(m => m.MName.ToUpper().Contains(keyword.ToUpper())).ToList();
            }
            var count = marks.Count();
            marks = marks.OrderBy(m => m.ID)
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
            return View(marks);
        }

        public ActionResult Create(decimal lat = 0, decimal lng = 0)
        {
            Mark mark = new Mark();
            mark.MLatitude = lat;
            mark.MLongitude = lng;
            CreateAction();
            return View(mark);
        }

        // POST: Role/Create
        [HttpPost]
        public ActionResult Create(Mark mark)
        {
            //JsonResult ret = new JsonResult();
            //try
            //{
            //    AddModelToContext(mark);
            //    db.SaveChanges();
            //    ret.Data = JsonConvert.SerializeObject(new
            //    {
            //        status = 0,
            //        message = "",
            //        data = mark
            //    });
            //}
            //catch (Exception ex)
            //{
            //    ret.Data = JsonConvert.SerializeObject(new
            //    {
            //        status = 1,
            //        message = ex.Message,
            //        data = mark
            //    });
            //}
            //return ret;
            return CreateModel(mark);
        }

        public ActionResult Edit(int id)
        {
            var mark = db.Mark.Find(id);
            return View("Create", mark);
        }
        [HttpPost]
        public ActionResult Edit(Mark mark)
        {
            EditAction();
            return EditModel(mark);
        }
        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            return DeleteModel<Mark>(ids);
        }
        public ActionResult Details(int id)
        {
            DetailsAction();
            var mark = db.Mark.Find(id);
            return View("Create", mark);
        }
    }
}