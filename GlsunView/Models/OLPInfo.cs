using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlsunView.CommService;

namespace GlsunView.Models
{
    public class OLPInfo
    {
        /// <summary>
        /// 工作模式
        /// </summary>
        public int Work_Mode { get; set; }
        /// <summary>
        /// 当前通道
        /// </summary>
        public int Current_Channel { get; set; }
        /// <summary>
        /// R1光功率
        /// </summary>
        public double R1_Input_Power { get; set; }
        /// <summary>
        /// R2光功率
        /// </summary>
        public double R2_Input_Power { get; set; }
        /// <summary>
        /// TX光功率
        /// </summary>
        public double TX_Input_Power { get; set; }
        /// <summary>
        /// ALM状态
        /// </summary>
        public int ALM_State { get; set; }
        /// <summary>
        /// R1状态
        /// </summary>
        public int R1_State { get; set; }
        /// <summary>
        /// R2状态
        /// </summary>
        public int R2_State { get; set; }
        /// <summary>
        /// TX状态
        /// </summary>
        public int TX_State { get; set; }
        /// <summary>
        /// LS状态
        /// </summary>
        public int LS_State { get; set; }
        /// <summary>
        /// 软件版本号
        /// </summary>
        public string Software_Version { get; set; }
        /// <summary>
        /// 硬件版本号
        /// </summary>
        public string hardware_Version { get; set; }
        /// <summary>
        /// 生产序列号
        /// </summary>
        public string Serial_Number { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public string Manufacture_Date { get; set; }
        /// <summary>
        /// R1波长[nm]
        /// </summary>
        public int R1_Wave { get; set; }
        /// <summary>
        /// R2波长[nm]
        /// </summary>
        public int R2_Wave { get; set; }
        /// <summary>
        /// Tx波长[nm]
        /// </summary>
        public int Tx_Wave { get; set; }
        /// <summary>
        /// R1切换门限(-50~+23) [dBm]
        /// </summary>
        public double R1_Switching_Power { get; set; }
        /// <summary>
        /// R2切换门限(-50~+23) [dBm]
        /// </summary>
        public double R2_Switching_Power { get; set; }
        /// <summary>
        /// R1告警门限(-50~+23) [dBm]
        /// </summary>
        public double R1_Alarm_Power { get; set; }
        /// <summary>
        /// R2告警门限(-50~+23) [dBm]
        /// </summary>
        public double R2_Alarm_Power { get; set; }
        /// <summary>
        /// TX告警门限(-50~+23) [dBm]
        /// </summary>
        public double TX_Alarm_Power { get; set; }
        /// <summary>
        /// 回切模式
        /// </summary>
        public int Back_Mode { get; set; }
        /// <summary>
        /// 自动回切延时(0~999分)
        /// </summary>
        public int Auto_Back_Delay { get; set; }
        /// <summary>
        /// 切换延时 (0~999秒)
        /// </summary>
        public int Switch_Delay { get; set; }
        /// <summary>
        /// 手动通道配置
        /// </summary>
        public int Manual_Switch_Channel { get; set; }
        /// <summary>
        /// 手动返回自动延时
        /// </summary>
        public int Manual_Back_to_Auto_Delay { get; set; }
        /// <summary>
        /// 工作模式关机保存
        /// </summary>
        public int Power_Off_Keep_Work_Mode { get; set; }

        public void RefreshData(CardCommService service)
        {
            var data = service.GetCardDataInfo();
            if (data.Contains("NOCARD")) return;
            Work_Mode = int.Parse(InstructionHelper.ExtractDataFromSet(data, "M"));
            Current_Channel = int.Parse(InstructionHelper.ExtractDataFromSet(data, "S"));
            R1_Input_Power = double.Parse(InstructionHelper.ExtractDataFromSet(data, "R1_P"));
            R2_Input_Power = double.Parse(InstructionHelper.ExtractDataFromSet(data, "R2_P"));
            TX_Input_Power = double.Parse(InstructionHelper.ExtractDataFromSet(data, "TX_P"));
            Software_Version = InstructionHelper.ExtractDataFromSet(data, "SV");
            hardware_Version = InstructionHelper.ExtractDataFromSet(data, "HV");
            Serial_Number = InstructionHelper.ExtractDataFromSet(data, "SN");
            Manufacture_Date = InstructionHelper.ExtractDataFromSet(data, "MD");
            R1_Wave = int.Parse(InstructionHelper.ExtractDataFromSet(data, "R1_W"));
            R2_Wave = int.Parse(InstructionHelper.ExtractDataFromSet(data, "R2_W"));
            Tx_Wave = int.Parse(InstructionHelper.ExtractDataFromSet(data, "TX_W"));
            R1_Switching_Power = double.Parse(InstructionHelper.ExtractDataFromSet(data, "R1_SP"));
            R2_Switching_Power = double.Parse(InstructionHelper.ExtractDataFromSet(data, "R2_SP"));
            R1_Alarm_Power = double.Parse(InstructionHelper.ExtractDataFromSet(data, "R1_AP"));
            R2_Alarm_Power = double.Parse(InstructionHelper.ExtractDataFromSet(data, "R2_AP"));
            TX_Alarm_Power = double.Parse(InstructionHelper.ExtractDataFromSet(data, "TX_AP"));
            Back_Mode = int.Parse(InstructionHelper.ExtractDataFromSet(data, "ACC"));
            Auto_Back_Delay = int.Parse(InstructionHelper.ExtractDataFromSet(data, "Q"));
            Switch_Delay = int.Parse(InstructionHelper.ExtractDataFromSet(data, "Y"));
            Manual_Switch_Channel = int.Parse(InstructionHelper.ExtractDataFromSet(data, "S"));
            Manual_Back_to_Auto_Delay = int.Parse(InstructionHelper.ExtractDataFromSet(data, "R"));
            Power_Off_Keep_Work_Mode = int.Parse(InstructionHelper.ExtractDataFromSet(data, "WMH"));

            //
            string almState = InstructionHelper.ExtractDataFromSet(data, "ALM");
            if (almState.Length == 5)
            {
                ALM_State = Convert.ToInt32(almState[0].ToString());
                R1_State = Convert.ToInt32(almState[1].ToString());
                R2_State = Convert.ToInt32(almState[2].ToString());
                TX_State = Convert.ToInt32(almState[3].ToString());
                LS_State = Convert.ToInt32(almState[4].ToString());
            }
        }
    }
}
