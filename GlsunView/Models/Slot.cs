using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlsunView.Models
{
    public class Slot
    {
        /// <summary>
        /// 槽位
        /// </summary>
        public int SlotNumber { get; set; }
        /// <summary>
        /// 是否插入
        /// </summary>
        public bool IsInsert { get; set; }
        /// <summary>
        /// 插卡类型
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 卡信息
        /// </summary>
        public object CardInfo { get; set; }
    }
}
