using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace GlsunView.Models
{
    public class AuthorityTreeNode
    {
        /// <summary>
        /// 显示节点ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 显示父节点ID
        /// </summary>
        public int PID { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Checked { get; set; }
        /// <summary>
        /// 数据源ID
        /// </summary>
        public int DataID { get; set; }
        /// <summary>
        /// 转换为JSON对象
        /// </summary>
        /// <returns></returns>
        public string ToJSONObject()
        {
            StringBuilder sbText = new StringBuilder();
            sbText.Append("{");
            sbText.AppendFormat("\"id\":{0}", ID);
            sbText.AppendFormat(",\"pId\":{0}", PID);
            sbText.AppendFormat(",\"name\":\"{0}\"", Name);
            sbText.AppendFormat(",\"dataId\":{0}", DataID);
            if (this.Checked)
            {
                sbText.Append(",\"checked\":true");
            }
            else
            {
                sbText.Append(",\"checked\":false");
            }
            sbText.Append("}");
            return sbText.ToString();
        }
    }
}