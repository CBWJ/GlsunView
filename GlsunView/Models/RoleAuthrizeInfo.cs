using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlsunView.Models
{
    public class RoleAuthrizeInfo
    {
        public int RoleId { get; set; }
        public List<string> UnCheckedId { get; set; }
        public List<string> CheckedId { get; set; }
    }
}