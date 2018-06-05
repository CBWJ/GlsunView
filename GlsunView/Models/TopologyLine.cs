using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlsunView.Models
{
    public class TopologyLine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int NodeIDA { get; set; }
        public int NodeIDZ { get; set; }
    }
}