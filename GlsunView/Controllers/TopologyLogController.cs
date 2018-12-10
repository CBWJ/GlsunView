using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;

namespace GlsunView.Controllers
{
    public class TopologyLogController : ShareListController
    {
        // GET: TopologyLog
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(int page = 1, int pageSize = 10)
        {
            LogSearchConditions conditions = null;
            IEnumerable<TopologyOperationLog> logs = null;
            PagingInfo pagingInfo = null;
            using (var ctx = new GlsunViewEntities())
            {
                logs = ctx.TopologyOperationLog.ToList();
            }
            conditions = (LogSearchConditions)Session["TopologyLogSearchConditions"];
            if (conditions == null)
            {
                conditions = new LogSearchConditions
                {
                    OperationType = "不限",
                    OperationResult = "不限",
                    OperationDateBeg = DateTime.Now.AddMonths(-3),
                    OperationDateEnd = DateTime.Now
                };
            }
            //筛选条件
            if (!string.IsNullOrWhiteSpace(conditions.Operator))
            {
                logs = logs.Where(d => d.ULoginName.Contains(conditions.Operator) || d.UName.Contains(conditions.Operator));
            }
            if (conditions.OperationType != "不限")
            {
                logs = logs.Where(d => d.TOLOperationType == conditions.OperationType);
            }
            if (conditions.OperationResult != "不限")
            {
                logs = logs.Where(d => d.TOLOperationResult == conditions.OperationResult);
            }
            logs = logs.Where(d => d.TOLOperationTime >= conditions.OperationDateBeg);
            logs = logs.Where(d => d.TOLOperationTime < conditions.OperationDateEnd.AddDays(1));
            var totalLogs = logs.Count();
            logs = logs.OrderByDescending(l => l.TOLOperationTime)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            pagingInfo = new PagingInfo
            {
                TotalItems = totalLogs,
                CurrentPage = page,
                ItemsPerPage = pageSize,
                ShowPageCount = 5
            };
            ViewBag.Conditions = conditions;
            ViewBag.PagingInfo = pagingInfo;
            SetAuthorityData();
            return View(logs);
        }
        [HttpPost]
        public ActionResult List(LogSearchConditions conditions)
        {
            if (conditions != null)
                Session["TopologyLogSearchConditions"] = conditions;
            return List();
        }
    }
}