using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlsunView.Infrastructure.Abstract;
using GlsunView.Domain;

namespace GlsunView.Infrastructure.Concrete
{
    public class Userlogger : IUserlogger
    {
        public void RecordLogin(string userName, string operation, string result, string remark)
        {
            using(var ctx = new GlsunViewEntities())
            {
                var user = (from u in ctx.User
                        where u.ULoginName == userName
                            select u).FirstOrDefault();
                if(user != null)
                {
                    UserLog ulog = new UserLog();
                    ulog.ULOperatorID = user.ID;
                    ulog.ULOperator = user.ULoginName;
                    ulog.ULOperationType = operation;
                    ulog.ULOperationResult = result;
                    ulog.ULOperationTime = DateTime.Now;
                    ulog.ULRemark = remark;

                    ctx.UserLog.Add(ulog);
                }
                ctx.SaveChanges();
            }
        }

        public void RecordModify(User uOperator, User uModified, string operation, string opDetails, string result, string remark)
        {
            if (uOperator == null || uModified == null)
                return;
            using (var ctx = new GlsunViewEntities())
            {
                UserLog ulog = new UserLog();
                ulog.ULOperatorID = uOperator.ID;
                ulog.ULOperator = uOperator.ULoginName;
                ulog.ULOperationType = operation;
                ulog.ULOperationDetials = opDetails;
                ulog.ULOperationResult = result;
                ulog.ULOperationTime = DateTime.Now;
                ulog.UID = uModified.ID;
                ulog.ULoginName = uModified.ULoginName;
                ulog.UName = uModified.UName;
                ulog.ULRemark = remark;

                ctx.UserLog.Add(ulog);
                ctx.SaveChanges();
            }
        }
    }
}
