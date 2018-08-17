using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlsunView.Models
{
    public class SFPModule
    {
        /// <summary>
        /// 所在槽位
        /// </summary>
        public int SlotPosition { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 工作模式
        /// </summary>
        public int Work_Mode { get; set; }
        /// <summary>
        /// 发光控制
        /// </summary>
        public int Tx_Power_Control { get; set; }
        /// <summary>
        /// 发光功率 (dBm)
        /// </summary>
        public double Tx_Power { get; set; }
        /// <summary>
        /// 收光功率 (dBm)
        /// </summary>
        public double Rx_Power { get; set; }
        /// <summary>
        /// 模块波长 (nm)
        /// </summary>
        public double Module_Wave { get; set; }
        /// <summary>
        /// 模块传输距离 (Km)
        /// </summary>
        public double Transmission_Distance { get; set; }
        /// <summary>
        /// 模块速率 (Gbit/s)
        /// </summary>
        public double Transmission_Rate { get; set; }
        /// <summary>
        /// 模块温度 (摄氏度)
        /// </summary>
        public double Module_Temperature { get; set; }
        /// <summary>
        /// 发光状态
        /// </summary>
        public int Tx_Power_State { get; set; }
        /// <summary>
        /// 收光状态
        /// </summary>
        public int Rx_Power_State { get; set; }
        /// <summary>
        /// 模块温度状态
        /// </summary>
        public int Module_Temperature_State { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 波道号
        /// </summary>
        public int Wave_Channel_Number { get; set; }
        /// <summary>
        /// 告警
        /// </summary>
        public string Alarm { get; set; }

    }
}
