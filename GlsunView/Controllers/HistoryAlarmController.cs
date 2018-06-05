using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;

namespace GlsunView.Controllers
{
    public class HistoryAlarmController : ShareListController
    {
        // GET: HistoryAlarm
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(int page = 1, int pageSize = 10)
        {
            AlarmQueryCondition conditions = null;
            IEnumerable<AlarmInformation> alarmInfo = null;
            //数据库取数据
            using (var ctx = new GlsunViewEntities())
            {
                alarmInfo = ctx.AlarmInformation.Where(a => a.AIConfirm == true).ToList();
            }
            //筛选
            conditions = (AlarmQueryCondition)Session["AlarmHistoryConditions"];
            if (conditions == null)
            {
                conditions = new AlarmQueryCondition
                {
                    IP = "",
                    AlarmLevel = "不限",
                    Confirmor = "",
                    ConfirmTimeBeg = DateTime.Now.AddMonths(-3),
                    ConfirmTimeEnd = DateTime.Now
                };
            }
            if (!string.IsNullOrWhiteSpace(conditions.IP))
            {
                alarmInfo = alarmInfo.Where(a => a.DAddress.Contains(conditions.IP));
            }
            if (conditions.AlarmLevel != "不限")
            {
                alarmInfo = alarmInfo.Where(a => a.AILevel == conditions.AlarmLevel);
            }
            if (!string.IsNullOrWhiteSpace(conditions.Confirmor))
            {
                alarmInfo = alarmInfo.Where(a => a.ULoginName.Contains(conditions.Confirmor) || a.UName.Contains(conditions.Confirmor));
            }
            alarmInfo = alarmInfo.Where(a => a.AIConfirmTime > conditions.ConfirmTimeBeg);
            alarmInfo = alarmInfo.Where(a => a.AIConfirmTime < conditions.ConfirmTimeEnd.AddDays(1));
            //分页处理
            var totals = alarmInfo.Count();
            alarmInfo = alarmInfo.OrderBy(a => a.AITime)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
            var pagingInfo = new PagingInfo
            {
                TotalItems = totals,
                CurrentPage = page,
                ItemsPerPage = pageSize,
                ShowPageCount = 5
            };
            ViewBag.PagingInfo = pagingInfo;
            ViewBag.Conditions = conditions;
            SetAuthorityData();
            return View(alarmInfo);
        }

        [HttpPost]
        public ActionResult List(AlarmQueryCondition conditions)
        {
            if (conditions != null)
                Session["AlarmHistoryConditions"] = conditions;
            return List();
        }
    }
}