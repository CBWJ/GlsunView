using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlsunView.Models
{
    /// <summary>
    /// 日志查询条件类
    /// </summary>
    public class LogSearchConditions
    {
        public string Operator { get; set; }
        public string OperationType { get; set; }
        public string OperationResult { get; set; }
        public DateTime OperationDateBeg { get; set; }
        public DateTime OperationDateEnd { get; set; }
    }
}