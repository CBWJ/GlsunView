using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlsunView.Models
{
    /// <summary>
    /// OEO设备视图显示模型
    /// 2018-8-16
    /// </summary>
    public class OEOViewModel
    {
        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 槽位
        /// </summary>
        public int Slot { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 工作方式
        /// </summary>
        public string WorkMode { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductModel { get; set; }
        /// <summary>
        /// 系列号
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// 硬件版本
        /// </summary>
        public string HardwareVersion { get; set; }
        /// <summary>
        /// 软件版本
        /// </summary>
        public string SoftwareVersion { get; set; }
    }
}