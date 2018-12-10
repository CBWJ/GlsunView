using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlsunView.CommService;

namespace GlsunView.Models
{
    public class EDFAInfo
    {
        /// <summary>
        /// 工作模式
        /// </summary>
        public int Work_Mode { get; set; }
        /// <summary>
        /// 泵浦开关
        /// </summary>
        public double PUMP_Switch { get; set; }
        /// <summary>
        /// 输入光功率
        /// </summary>
        public double Input_Power { get; set; }
        /// <summary>
        /// 输出光功率
        /// </summary>
        public double Output_Power { get; set; }
        /// <summary>
        /// 输入光告警门限
        /// </summary>
        public double Input_Power_Limit { get; set; }
        /// <summary>
        /// 输出光告警门限
        /// </summary>
        public double Output_Power_Limit { get; set; }
        /// <summary>
        /// 模块温度告警上限
        /// </summary>
        public double Modlue_Temperature_Upper_Limit { get; set; }
        /// <summary>
        /// 模块温度告警下限
        /// </summary>
        public double Modlue_Temperature_Lower_Limit { get; set; }
        /// <summary>
        /// 泵浦温度告警上限
        /// </summary>
        public double PUMP_Temperature_Upper_Limit { get; set; }
        /// <summary>
        /// 泵浦温度告警下限
        /// </summary>
        public double PUMP_Temperature_Lower_Limit { get; set; }
        /// <summary>
        /// 输入光状态
        /// </summary>
        public int Input_Power_State { get; set; }
        /// <summary>
        /// 输出光状态
        /// </summary>
        public int Output_Power_State { get; set; }
        /// <summary>
        /// 模块温度状态
        /// </summary>
        public int Modlue_Temperature_State { get; set; }
        /// <summary>
        /// 泵浦温度状态
        /// </summary>
        public int PUMP_Temperature_State { get; set; }
        /// <summary>
        /// 泵浦电流状态
        /// </summary>
        public int PUMP_Electric_Current_State { get; set; }
        /// <summary>
        /// 输出光功率配置
        /// </summary>
        public double Output_Power_Set { get; set; }
        /// <summary>
        /// 模块温度
        /// </summary>
        public double Modlue_Temperature { get; set; }
        /// <summary>
        /// 模块电压
        /// </summary>
        public double Modlue_Voltage { get; set; }
        /// <summary>
        /// 泵浦功率
        /// </summary>
        public double PUMP_Power { get; set; }
        /// <summary>
        /// 泵浦温度
        /// </summary>
        public double PUMP_Temperature { get; set; }
        /// <summary>
        /// 泵浦电流
        /// </summary>
        public double PUMP_Electric_Current { get; set; }
        /// <summary>
        /// TEC 电流
        /// </summary>
        public double TEC_Electric_Current { get; set; }
        /// <summary>
        /// 软件版本
        /// </summary>
        public string Software_Version { get; set; }
        /// <summary>
        /// 硬件版本
        /// </summary>
        public string Hardware_Version { get; set; }
        /// <summary>
        /// 生产序列号
        /// </summary>
        public string Serial_Number { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public string Manufacture_Date { get; set; }
        /// <summary>
        /// 设备类型描述
        /// </summary>
        public string Device_Type { get; set; }        
        /// <summary>
        /// 当前增益
        /// </summary>
        public double Current_Gain { get; set; }
        
        /// <summary>
        /// 增益设置值
        /// </summary>
        public double Gain_Setting { get; set; }
        /// <summary>
        /// 电流设置值
        /// </summary>
        public double Current_Setting { get; set; }
        /// <summary>
        /// 输出功率设置值
        /// </summary>
        public double Output_Power_Setting { get; set; }
        /// <summary>
        /// 关泵阈值
        /// </summary>
        public double Pump_Threshold { get; set; }
        /// <summary>
        /// 输入低阈值
        /// </summary>
        public double Input_Low_Threshold { get; set; }
        /// <summary>
        /// 输入高阈值
        /// </summary>
        public double Input_high_Threshold { get; set; }

        public void RefreshData(CardCommService service)
        {
            var arrBasic = InstructionHelper.ExtractData(service.GetCardBasicInfo(), "B").Split('_');
            if(arrBasic.Length == 5)
            {
                Software_Version = arrBasic[3];
                Hardware_Version = arrBasic[4];
            }
            var data = service.GetCardDataInfo();
            if (data.Contains("NOCARD")) return;
            Work_Mode = int.Parse(InstructionHelper.ExtractDataFromSet(data, "M"));
            PUMP_Switch = double.Parse(InstructionHelper.ExtractDataFromSet(data, "PSW"));
            Input_Power = double.Parse(InstructionHelper.ExtractDataFromSet(data, "PWI"));
            Output_Power = double.Parse(InstructionHelper.ExtractDataFromSet(data, "PWO"));
            Input_Power_Limit = double.Parse(InstructionHelper.ExtractDataFromSet(data, "PIA"));
            Output_Power_Limit = double.Parse(InstructionHelper.ExtractDataFromSet(data, "POA"));
            Modlue_Temperature_Upper_Limit = double.Parse(InstructionHelper.ExtractDataFromSet(data, "MTU"));
            Modlue_Temperature_Lower_Limit = double.Parse(InstructionHelper.ExtractDataFromSet(data, "MTD"));
            PUMP_Temperature_Upper_Limit = double.Parse(InstructionHelper.ExtractDataFromSet(data, "PTU"));
            PUMP_Temperature_Lower_Limit = double.Parse(InstructionHelper.ExtractDataFromSet(data, "PTD"));
            Input_Power_State = int.Parse(InstructionHelper.ExtractDataFromSet(data, "PIN"));
            Output_Power_State = int.Parse(InstructionHelper.ExtractDataFromSet(data, "POU"));
            Modlue_Temperature_State = int.Parse(InstructionHelper.ExtractDataFromSet(data, "MT"));
            PUMP_Temperature_State = int.Parse(InstructionHelper.ExtractDataFromSet(data, "PT"));
            PUMP_Electric_Current_State = int.Parse(InstructionHelper.ExtractDataFromSet(data, "PI"));
            //Output_Power_Set = double.Parse(InstructionHelper.ExtractDataFromSet(data, ""));
            Modlue_Temperature = double.Parse(InstructionHelper.ExtractDataFromSet(data, "MTV"));
            Modlue_Voltage = double.Parse(InstructionHelper.ExtractDataFromSet(data, "MPV"));
            PUMP_Power = double.Parse(InstructionHelper.ExtractDataFromSet(data, "PPV"));
            PUMP_Temperature = double.Parse(InstructionHelper.ExtractDataFromSet(data, "PTV"));
            //PUMP_Electric_Current = double.Parse(InstructionHelper.ExtractDataFromSet(data, ""));
            TEC_Electric_Current = double.Parse(InstructionHelper.ExtractDataFromSet(data, "TEC"));
            //Software_Version = InstructionHelper.ExtractDataFromSet(data, "SV");
            //Hardware_Version = InstructionHelper.ExtractDataFromSet(data, "HV");
            Serial_Number = InstructionHelper.ExtractDataFromSet(data, "SN");
            Manufacture_Date = InstructionHelper.ExtractDataFromSet(data, "MD");
            Device_Type = InstructionHelper.ExtractDataFromSet(data, "DT");
            Current_Gain = double.Parse(InstructionHelper.ExtractDataFromSet(data, "CPGV"));
            //Set_Gain = double.Parse(InstructionHelper.ExtractDataFromSet(data, "PGV"));
        }
    }
}
