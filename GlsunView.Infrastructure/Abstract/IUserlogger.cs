using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlsunView.Domain;

namespace GlsunView.Infrastructure.Abstract
{
    public interface IUserlogger
    {
        void RecordLogin(string userName, string operation, string result, string remark);
        void RecordModify(User Operator, User Modified, string operation, string opDetails, string result, string remark);
    }
}
