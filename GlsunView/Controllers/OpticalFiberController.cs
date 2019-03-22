using GlsunView.Domain;
using GlsunView.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlsunView.Controllers
{
    public class OpticalFiberController : ShareListController
    {
        // GET: OpticalFiber
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(int page = 1, int pageSize = 10, string keyword = "")
        {
            IEnumerable<OpticalFiber> fibers = db.OpticalFiber.ToList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                fibers = fibers.Where(m => m.OC_Name.ToUpper().Contains(keyword.ToUpper())).ToList();
            }
            var count = fibers.Count();
            fibers = fibers.OrderBy(m => m.ID)
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
            return View(fibers);
        }
    }
}