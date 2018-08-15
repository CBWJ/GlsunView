using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlsunView.Models
{
    public class DeviceInfo
    {
        /// <summary>
        /// 型号
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 机架
        /// </summary>
        public string Shelf { get; set; }
        /// <summary>
        /// 机房
        /// </summary>
        public string Room { get; set; }
        /// <summary>
        /// 地点
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 系列号
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 子网掩码
        /// </summary>
        public string Mask { get; set; }
        /// <summary>
        /// MAC地址
        /// </summary>
        public string MACAddr { get; set; }

    }
}