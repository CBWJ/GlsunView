//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace GlsunView.Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class TopologyOperationLog
    {
        public int ID { get; set; }
        public Nullable<int> UID { get; set; }
        public string ULoginName { get; set; }
        public string UName { get; set; }
        public string TOLOperationType { get; set; }
        public string TOLOperationDetials { get; set; }
        public string TOLOperationResult { get; set; }
        public Nullable<System.DateTime> TOLOperationTime { get; set; }
        public string Remark { get; set; }
        public Nullable<int> TOLObjectID { get; set; }
        public string TOLObjectName { get; set; }
        public string TOLObjectType { get; set; }
    }
}
