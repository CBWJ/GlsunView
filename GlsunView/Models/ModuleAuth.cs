using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GlsunView.Domain;

namespace GlsunView.Models
{
    public class ModuleAuth
    {
        public int ModuleID { get; set; }
        public IEnumerable<Authority> Authorities { get; set; }
        public List<int> OwnAuthorityID { get; set; }
    }
}