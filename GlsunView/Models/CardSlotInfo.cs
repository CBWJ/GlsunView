using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlsunView.Models
{
    public class CardSlotInfo
    {
        /// <summary>
        /// 槽位
        /// </summary>
        public int Slot { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 工作模式
        /// </summary>
        public string WorkMode { get; set; }
        /// <summary>
        /// 硬件版本
        /// </summary>
        public string HardwareVersion { get; set; }
        /// <summary>
        /// 软件版本
        /// </summary>
        public string SoftwareVersion { get; set; }
        /// <summary>
        /// 当前告警
        /// </summary>
        public string CurrentAlarm { get; set; }

    }
}