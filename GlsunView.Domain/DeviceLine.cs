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
    
    public partial class DeviceLine
    {
        public int ID { get; set; }
        public string DLName { get; set; }
        public Nullable<int> DIDA { get; set; }
        public Nullable<int> DIDB { get; set; }
        public Nullable<int> CreatorID { get; set; }
        public Nullable<System.DateTime> CreationTime { get; set; }
        public Nullable<int> EditorID { get; set; }
        public Nullable<System.DateTime> EditingTime { get; set; }
    }
}
