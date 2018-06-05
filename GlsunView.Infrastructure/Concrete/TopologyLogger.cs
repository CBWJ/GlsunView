using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlsunView.Domain;

namespace GlsunView.Infrastructure.Concrete
{
    public class TopologyLogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">操作用户</param>
        /// <param name="operation">操作类型</param>
        /// <param name="opDetails">操作明细</param>
        /// <param name="result">操作结果</param>
        /// <param name="remark">备注</param>
        /// <param name="objId">操作对象ID</param>
        /// <param name="objName">操作对象名称</param>
        /// <param name="objType">操作对象类型</param>
        public void Record(User user, string operation, string opDetails, string result, string remark, int objId, string objName, string objType)
        {
            if (user == null )
                return;
            using (var ctx = new GlsunViewEntities())
            {
                var log = ctx.TopologyOperationLog.Create();
                log.UID = user.ID; //操作者ID
                log.ULoginName = user.ULoginName;  //操作者登陆名
                log.UName = user.UName;   //操作者用户名
                log.TOLOperationType = operation;    //操作类型
                log.TOLOperationDetials = opDetails; //操作明细
                log.TOLOperationResult = result;  //操作结果
                log.TOLOperationTime = DateTime.Now;    //操作时间
                log.Remark = remark;  //备注
                log.TOLObjectID = objId; //操作对象ID
                log.TOLObjectName = objName;   //操作对象名称
                log.TOLObjectType = objType;	//操作对象类型
                ctx.TopologyOperationLog.Add(log);
                ctx.SaveChanges();
            }
        }
    }
}
