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
    
    public partial class Route
    {
        public int ID { get; set; }
        public string RName { get; set; }
        public string RType { get; set; }
        public Nullable<int> RGID { get; set; }
        public string RAName { get; set; }
        public Nullable<int> RAMFID { get; set; }
        public string RAIP { get; set; }
        public Nullable<short> RASlot { get; set; }
        public string RBName { get; set; }
        public Nullable<int> RBMFID { get; set; }
        public string RBIP { get; set; }
        public Nullable<short> RBSlot { get; set; }
        public string Remark { get; set; }
        public Nullable<int> CreatorID { get; set; }
        public Nullable<System.DateTime> CreationTime { get; set; }
        public Nullable<int> EditorID { get; set; }
        public Nullable<System.DateTime> EditingTime { get; set; }
    
        public virtual RouteGroup RouteGroup { get; set; }
    }
}