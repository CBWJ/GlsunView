using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlsunView.Models
{
    /// <summary>
    /// 拓扑节点
    /// </summary>
    public class TopologyNode
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Icon { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}