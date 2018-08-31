using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlsunView.Models
{
    public class RouteViewModel
    {
        /// <summary>
        /// 路由名称
        /// </summary>
        public string RouteName { get; set; }
        /// <summary>
        /// A端名称
        /// </summary>
        public string AName { get; set; }
        /// <summary>
        /// A端IP
        /// </summary>
        public string AIP { get; set; }
        /// <summary>
        /// A端端口
        /// </summary>
        public int APort { get; set; }
        /// <summary>
        /// A端卡槽
        /// </summary>
        public int ASlot { get; set; }
        /// <summary>
        /// A端卡位置
        /// </summary>
        public string ACardPosition { get; set; }
        /// <summary>
        /// A端卡类型
        /// </summary>
        public string ACardType { get; set; }
        /// <summary>
        /// A工作线路
        /// </summary>
        public int AWorkRoute { get; set; }
        /// <summary>
        /// B端名称
        /// </summary>
        public string BName { get; set; }
        /// <summary>
        /// B端IP
        /// </summary>
        public string BIP { get; set; }
        /// <summary>
        /// B端端口
        /// </summary>
        public int BPort { get; set; }
        /// <summary>
        /// B端卡槽
        /// </summary>
        public int BSlot { get; set; }
        /// <summary>
        /// B端卡位置
        /// </summary>
        public string BCardPosition { get; set; }
        /// <summary>
        /// B端卡类型
        /// </summary>
        public string BCardType { get; set; }
        /// <summary>
        /// B工作线路
        /// </summary>
        public int BWorkRoute { get; set; }

    }
}