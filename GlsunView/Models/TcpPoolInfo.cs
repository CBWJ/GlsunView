using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlsunView.Models
{
    public class TcpPoolInfo
    {
        /// <summary>
        /// 客户端
        /// </summary>
        public string Client { get; set; }
        /// <summary>
        /// 连接总数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 空闲数
        /// </summary>
        public int FreeCount { get; set; }
        /// <summary>
        /// 忙碌数
        /// </summary>
        public int BusyCount { get; set; }
    }
}