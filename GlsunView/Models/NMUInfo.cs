using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlsunView.CommService;

namespace GlsunView.Models
{
    public class NMUInfo
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP_Address { get; set; }
        /// <summary>
        /// 默认网关
        /// </summary>
        public string Gateway { get; set; }
        /// <summary>
        /// 子网掩码
        /// </summary>
        public string Subnet_Mask { get; set; }
        /// <summary>
        /// Trap IP地址 1
        /// </summary>
        public string Trap_IP_1 { get; set; }
        /// <summary>
        /// Trap IP地址 2
        /// </summary>
        public string Trap_IP_2 { get; set; }
        /// <summary>
        /// Trap IP地址 3
        /// </summary>
        public string Trap_IP_3 { get; set; }
        /// <summary>
        /// 电源1状态
        /// </summary>
        public int Power_1_Status { get; set; }
        /// <summary>
        /// 电源2状态
        /// </summary>
        public int Power_2_Status { get; set; }
        /// <summary>
        /// 风扇状态
        /// </summary>
        public int FAN_Status { get; set; }
        /// <summary>
        /// 共用体(读)
        /// </summary>
        public string Community_Read { get; set; }
        /// <summary>
        /// 共用体(读/写)
        /// </summary>
        public string Community_Write { get; set; }
        /// <summary>
        /// 软件版本号
        /// </summary>
        public string Software_Version { get; set; }
        /// <summary>
        /// 硬件版本号
        /// </summary>
        public string Hardware_Version { get; set; }
        /// <summary>
        /// 生产序列号
        /// </summary>
        public string Serial_Number { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public string Manufacturing_Date { get; set; }
        /// <summary>
        /// 管理员地址
        /// </summary>
        public string SysLocation { get; set; }
        /// <summary>
        /// 管理员姓名
        /// </summary>
        public string SysName { get; set; }
        /// <summary>
        /// 管理员联系方式
        /// </summary>
        public string SysContact { get; set; }

        public void RefreshStatus(NMUCommService service)
        {
            //单一数据处理
            IP_Address = InstructionHelper.ExtractData(service.GetIPAddress(), "IP");
            Gateway = InstructionHelper.ExtractData(service.GetGateway(), "GW");
            Subnet_Mask = InstructionHelper.ExtractData(service.GetNetMask(), "MSK");
            //FAN_Status = Convert.ToInt32(InstructionHelper.ExtractData(service.GetFanWorkingStatus(), "FNS"));
            int fan_state = 0;
            int.TryParse(InstructionHelper.ExtractData(service.GetFanWorkingStatus(), "FNS"), out fan_state);
            FAN_Status = fan_state;
            Software_Version = InstructionHelper.ExtractData(service.GetDeviceSoftwareVersion(), "SV");
            Hardware_Version = InstructionHelper.ExtractData(service.GetDeviceHardwareVersion(), "HV");
            Serial_Number = InstructionHelper.ExtractData(service.GetDeviceProductionSerialNumber(), "SN");
            Manufacturing_Date = InstructionHelper.ExtractData(service.GetDeviceFactoryDate(), "MD");
            //多重数据处理
            var powerStatus = InstructionHelper.ExtractData(service.GetPowerWorkingStatus(), "PWR");
            if (powerStatus.Length == 2)
            {
                Power_1_Status = Convert.ToInt32(powerStatus.Substring(0, 1));
                Power_2_Status = Convert.ToInt32(powerStatus.Substring(1, 1));
            }
        }
    }
}
