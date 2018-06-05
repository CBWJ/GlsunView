using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlsunView.Models
{
    public class AlarmQueryCondition
    {
        public string IP { get; set; }
        public string AlarmLevel { get; set; }
        /// <summary>
        /// 告警时间
        /// </summary>
        public DateTime AlarmTimeBeg { get; set; }
        public DateTime AlarmTimeEnd { get; set; }
        /// <summary>
        /// 确认人
        /// </summary>
        public string Confirmor { get; set; }
        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTime ConfirmTimeBeg { get; set; }
        public DateTime ConfirmTimeEnd { get; set; }
    }
}