﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlsunView.Domain;
using GlsunView.Models;
using GlsunView.Infrastructure.Util;

namespace GlsunView.Controllers
{
    public class CurrentAlarmController : ShareListController
    {
        // GET: CurrentAlram
        public ActionResult Index()
        {
            IEnumerable<v_AlarmInfo> alarmInfo = null;
            //数据库取数据
            using (var ctx = new GlsunViewEntities())
            {
                alarmInfo = ctx.v_AlarmInfo.Where(a => a.AIConfirm == false).ToList();
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
            using(var ctx = new GlsunViewEntities())
            {
                alarmInfo = ctx.AlarmInformation
                    .Where(a => a.AIConfirm == false)                    
                    .ToList();
            }
            //筛选
            conditions = (AlarmQueryCondition)Session["AlarmQueryConditions"];
            if(conditions == null)
            {
                conditions = new AlarmQueryCondition
                {
                    IP = "",
                    AlarmLevel = "不限",
                    AlarmTimeBeg = DateTime.Now.AddMonths(-3),
                    AlarmTimeEnd = DateTime.Now
                };
            }
            if (!string.IsNullOrWhiteSpace(conditions.IP))
            {
                alarmInfo = alarmInfo.Where(a => a.DAddress.Contains(conditions.IP));
            }
            if(conditions.AlarmLevel != "不限")
            {
                alarmInfo = alarmInfo.Where(a => a.AILevel == conditions.AlarmLevel);
            }
            alarmInfo = alarmInfo.Where(a => a.AITime > conditions.AlarmTimeBeg);
            alarmInfo = alarmInfo.Where(a => a.AITime < conditions.AlarmTimeEnd.AddDays(1));
            //分页处理
            var totals = alarmInfo.Count();
            alarmInfo = alarmInfo.OrderByDescending(a => a.AITime)
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
                Session["AlarmQueryConditions"] = conditions;
            return List();
        }
        [HttpPost]
        public ActionResult Confirm(List<int> checkedIds)
        {
            using(var ctx = new GlsunViewEntities())
            {
                var loginUser = (from u in ctx.User
                                 where u.ULoginName == HttpContext.User.Identity.Name
                                 select u).FirstOrDefault();
                var alarmInfos = ctx.AlarmInformation.Where(a => checkedIds.Contains(a.ID));
                foreach(var info in alarmInfos)
                {
                    info.AIConfirm = true;
                    info.AIConfirmTime = DateTime.Now;
                    info.UID = loginUser.ID;
                    info.ULoginName = loginUser.ULoginName;
                    info.UName = loginUser.UName;
                }
                ctx.SaveChanges();
            }
            return RedirectToAction("List");
        }
        public ActionResult ConfirmFast(string confirmIds)
        {
            JsonResult json = new JsonResult();
            var arrIds = confirmIds.Split(',');
            List<int> Ids = new List<int>();
            if (arrIds.Length > 0)
            {
                Ids = (from id in arrIds
                           select int.Parse(id)).ToList();
                using (var ctx = new GlsunViewEntities())
                {
                    var loginUser = (from u in ctx.User
                                     where u.ULoginName == HttpContext.User.Identity.Name
                                     select u).FirstOrDefault();
                    var alarmInfos = ctx.AlarmInformation.Where(a => Ids.Contains(a.ID));
                    foreach (var info in alarmInfos)
                    {
                        info.AIConfirm = true;
                        info.AIConfirmTime = DateTime.Now;
                        info.UID = loginUser.ID;
                        info.ULoginName = loginUser.ULoginName;
                        info.UName = loginUser.UName;
                    }
                    ctx.SaveChanges();
                }
            }
            json.Data = Ids;
            return json;
        }
        public ActionResult CardAlarm(int did, int slot)
        {
            IEnumerable<AlarmInformation> alarmInfo = null;
            //数据库取数据
            using (var ctx = new GlsunViewEntities())
            {
                alarmInfo = ctx.AlarmInformation.Where(a => a.DID == did && a.AISlot == slot).ToList();
            }
            alarmInfo = (from a in alarmInfo
                         orderby a.AITime descending
                         select a).ToList();
            return View(alarmInfo);
        }
        /// <summary>
        /// 获取实时告警信息
        /// </summary>
        /// <param name="level"></param>
        /// <param name="exceptIds"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RealTimeAlarm(string level, string exceptIds)
        {
            JsonResult json = new JsonResult();
            IEnumerable<v_AlarmInfo> alarmInfo = null;
            alarmInfo = MemoryCacheHelper.GetCacheItem<IEnumerable<v_AlarmInfo>>("curr_alarm",
                () => 
                {
                    IEnumerable<v_AlarmInfo> infos = null;
                    //数据库取数据
                    using (var ctx = new GlsunViewEntities())
                    {
                        infos = ctx.v_AlarmInfo.Where(a => a.AIConfirm == false).ToList();
                    }
                    return infos;
                },
                null, DateTime.Now.AddSeconds(2));
            List<int> ids = new List<int>();
            if (alarmInfo != null)
            {
                ids = alarmInfo.Select(a => a.ID).ToList();
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
            }
            else
            {
                //throw new NullReferenceException("从缓存取当前告警为NULL");
            }
            json.Data = new { Alarm = alarmInfo, IDSet = ids};
            return json;
            //return JsonHelper.ObjectToJson(alarmInfo);
        }
    }
}