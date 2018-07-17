using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;
using GlsunView.Infrastructure.Util;

namespace GlsunView.Controllers
{
    public class HistoryAlarmController : ShareListController
    {
        // GET: HistoryAlarm
        public ActionResult Index()
        {
            IEnumerable<v_AlarmInfo> alarmInfo = null;
            //数据库取数据
            using (var ctx = new GlsunViewEntities())
            {
                alarmInfo = ctx.v_AlarmInfo.Where(a => a.AIConfirm == true).ToList();
            }
            alarmInfo = alarmInfo.OrderByDescending(a => a.AITime);
            ViewBag.Level = "";
            return View(alarmInfo);
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
                    AlarmTimeBeg = DateTime.Now.AddMonths(-3),
                    AlarmTimeEnd = DateTime.Now,
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
            alarmInfo = alarmInfo.Where(a => a.AITime > conditions.AlarmTimeBeg);
            alarmInfo = alarmInfo.Where(a => a.AITime < conditions.AlarmTimeEnd.AddDays(1));
            //alarmInfo = alarmInfo.Where(a => a.AIConfirmTime > conditions.ConfirmTimeBeg);
            //alarmInfo = alarmInfo.Where(a => a.AIConfirmTime < conditions.ConfirmTimeEnd.AddDays(1));
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

        public ActionResult RealTimeAlarm(string exceptIds)
        {
            JsonResult json = new JsonResult();
            IEnumerable<v_AlarmInfo> alarmInfo = null;
            alarmInfo = MemoryCacheHelper.GetCacheItem<IEnumerable<v_AlarmInfo>>("hist_alarm",
                () =>
                {
                    IEnumerable<v_AlarmInfo> infos = null;
                    //数据库取数据
                    using (var ctx = new GlsunViewEntities())
                    {
                        infos = ctx.v_AlarmInfo.Where(a => a.AIConfirm == true).OrderByDescending(a => a.AITime).Take(1000).ToList();
                    }
                    return infos;
                },
                null, DateTime.Now.AddSeconds(2));

            //排除已显示的项
            if (!string.IsNullOrWhiteSpace(exceptIds))
            {
                var arrIds = exceptIds.Split(',');
                if (arrIds.Length > 0)
                {
                    var Ids = (from id in arrIds
                               select int.Parse(id)).ToList();
                    alarmInfo = alarmInfo.Where(a => !Ids.Contains(a.ID)).OrderBy(a => a.AITime);
                }
            }
            json.Data = alarmInfo;
            return json;
        }
    }
}