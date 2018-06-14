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
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            IEnumerable<v_LastestWeekAlarmStatistics> weekAlarm = null;
            using (var ctx = new GlsunViewEntities())
            {
                weekAlarm = ctx.v_LastestWeekAlarmStatistics.ToList();
            }
            ViewBag.WeekAlarm = JsonHelper.getJsonByObject(weekAlarm);
            return View();
        }
    }
}