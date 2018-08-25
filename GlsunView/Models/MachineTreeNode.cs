using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GlsunView.Models
{
    public class MachineTreeNode : AuthorityTreeNode
    {
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 是否打开
        /// </summary>
        public bool Open { get; set; }
        /// <summary>
        /// 图标路径
        /// </summary>
        public string IconPath { get; set; }
        /// <summary>
        /// 转换为JSON对象
        /// </summary>
        /// <returns></returns>
        public override string ToJSONObject()
        {
            StringBuilder sbText = new StringBuilder();
            sbText.Append("{");
            sbText.AppendFormat("\"id\":{0}", ID);
            sbText.AppendFormat(",\"pId\":{0}", PID);
            sbText.AppendFormat(",\"name\":\"{0}\"", Name);
            sbText.AppendFormat(",\"dataId\":{0}", DataID);
            sbText.AppendFormat(",\"open\":{0}", Open.ToString().ToLower());
            if (!string.IsNullOrWhiteSpace(Icon))
            {
                sbText.AppendFormat(",\"icon\":\"{0}{1}\"", IconPath, Icon);
            }
            sbText.Append("}");
            return sbText.ToString();
        }
    }
}