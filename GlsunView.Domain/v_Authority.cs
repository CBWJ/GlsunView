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
    
    public partial class v_Authority
    {
        public int ID { get; set; }
        public string AName { get; set; }
        public string ACode { get; set; }
        public string AType { get; set; }
        public string AIcon { get; set; }
        public string AIconType { get; set; }
        public string AClassName { get; set; }
        public Nullable<short> AShowNumber { get; set; }
        public Nullable<bool> AIsCommon { get; set; }
        public Nullable<bool> IsEnabled { get; set; }
        public Nullable<int> CreatorID { get; set; }
        public Nullable<System.DateTime> CreationTime { get; set; }
        public Nullable<int> EditorID { get; set; }
        public Nullable<System.DateTime> EditingTime { get; set; }
        public string Creator { get; set; }
        public string Editor { get; set; }
    }
}
