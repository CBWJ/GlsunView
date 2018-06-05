using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlsunView.Models
{
    public class DeviceStatusSet
    {
        public DeviceOverview Overview { get; set; }
        public NMUInfo NMUInfo { get; set; }
        public List<object> CardsInfo { get; set; }
    }
}