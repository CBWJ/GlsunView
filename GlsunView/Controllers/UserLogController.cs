using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;

namespace GlsunView.Controllers
{
    public class UserLogController : ShareListController
    {
        // GET: UserLog
        public ActionResult List(int page = 1, int pageSize = 10)
        {
            LogSearchConditions conditions = null;
            IEnumerable<UserLog> logs = null;
            PagingInfo pagingInfo = null;
            using (var ctx = new GlsunViewEntities())
            {
                //logs = ctx.UserLog.OrderBy(r => r.ID)
                //        .Skip((page - 1) * pageSize)
                //        .Take(pageSize)
                //        .ToList();
                logs = ctx.UserLog.ToList();
            }
            conditions = (LogSearchConditions)Session["UserLogSearchConditions"];
            if (conditions == null)
            {
                conditions = new LogSearchConditions
                {
                    Operator = "",
                    OperationType = "不限",
                    OperationResult = "不限",
                    OperationDateBeg = DateTime.Now.AddMonths(-3),
                    OperationDateEnd = DateTime.Now
                };
            }
            if (!string.IsNullOrWhiteSpace(conditions.Operator))
            {
                logs = logs.Where(u => u.ULOperator.Contains(conditions.Operator));
            }
            if(conditions.OperationType != "不限")
            {
                logs = logs.Where(u => u.ULOperationType == conditions.OperationType);
            }
            if(conditions.OperationResult != "不限")
            {
                logs = logs.Where(u => u.ULOperationResult == conditions.OperationResult);
            }
            logs = logs.Where(u => u.ULOperationTime >= conditions.OperationDateBeg);
            logs = logs.Where(u => u.ULOperationTime < conditions.OperationDateEnd.AddDays(1));
            var totalLogs = logs.Count();
            logs = logs.OrderBy(r => r.ID)
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
            if(conditions != null)
                Session["UserLogSearchConditions"] = conditions;
            return List();
        }
    }
}