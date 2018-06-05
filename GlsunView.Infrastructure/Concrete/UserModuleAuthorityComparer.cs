using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlsunView.Domain;
using System.Collections;

namespace GlsunView.Infrastructure.Concrete
{
    public class UserModuleAuthorityComparer : IEqualityComparer<v_UserModuleAuthority>
    {
        public bool Equals(v_UserModuleAuthority x, v_UserModuleAuthority y)
        {
            return x.ACode == y.ACode;
        }

        public int GetHashCode(v_UserModuleAuthority obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}
