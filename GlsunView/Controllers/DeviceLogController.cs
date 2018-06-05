using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;

namespace GlsunView.Controllers
{
    public class DeviceLogController : ShareListController
    {
        // GET: DeviceLog
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(int page = 1, int pageSize = 10)
        {
            LogSearchConditions conditions = null;
            IEnumerable<DeviceOperationLog> logs = null;
            PagingInfo pagingInfo = null;
            using (var ctx = new GlsunViewEntities())
            {
                logs = ctx.DeviceOperationLog.ToList();
            }
            conditions = (LogSearchConditions)Session["DeviceLogSearchConditions"];
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
                logs = logs.Where(d => d.DOLOperationType == conditions.OperationType);
            }
            if (conditions.OperationResult != "不限")
            {
                logs = logs.Where(d => d.DOLOperationResult == conditions.OperationResult);
            }
            logs = logs.Where(d => d.DOLOperationTime >= conditions.OperationDateBeg);
            logs = logs.Where(d => d.DOLOperationTime < conditions.OperationDateEnd.AddDays(1));
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
            if (conditions != null)
                Session["DeviceLogSearchConditions"] = conditions;
            return List();
        }

        public ActionResult CardLog(int did, int slot)
        {
            IEnumerable<DeviceOperationLog> logs = null;
            using(var ctx = new GlsunViewEntities())
            {
                var device = ctx.Device.Find(did);
                logs = (from l in ctx.DeviceOperationLog
                        where l.SID == device.SID && l.DID == did && l.DOLDeviceSlot == slot
                        select l).ToList();
            }
            return View(logs);
        }
    }
}