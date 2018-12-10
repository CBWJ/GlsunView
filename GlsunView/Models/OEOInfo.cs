using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlsunView.CommService;

namespace GlsunView.Models
{
    public class OEOInfo
    {
        public List<SFPModule> SFPSet { get; set; }
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
        public string Manufacture_Date { get; set; }

        public void RefreshData(CardCommService service)
        {
            var arrBasic = InstructionHelper.ExtractData(service.GetCardBasicInfo(), "B").Split('_');
            if (arrBasic.Length == 5)
            {
                Software_Version = arrBasic[3];
                Hardware_Version = arrBasic[4];
            }
            var data = service.GetCardDataInfo();
            if (data.Contains("NOCARD")) return;
            //Software_Version = InstructionHelper.ExtractDataFromSet(data, "SV");
            //Hardware_Version = InstructionHelper.ExtractDataFromSet(data, "HV");
            Serial_Number = InstructionHelper.ExtractDataFromSet(data, "SN");
            Manufacture_Date = InstructionHelper.ExtractDataFromSet(data, "MD");
            if(SFPSet == null)
            {
                SFPSet = new List<SFPModule>();
                for(int i = 1; i <= 8; ++i)
                {
                    SFPModule sfp = new SFPModule
                    {
                        SlotPosition = i
                    };
                    SFPSet.Add(sfp);
                }
            }
            foreach(var sfp in SFPSet)
            {
                sfp.Status = int.Parse(InstructionHelper.ExtractDataFromSet(data, string.Format("M{0:D2}_{1}", sfp.SlotPosition, "MS")));
                sfp.Work_Mode = int.Parse(InstructionHelper.ExtractDataFromSet(data, string.Format("M{0:D2}_{1}", sfp.SlotPosition, "M")));
                sfp.Tx_Power_Control = int.Parse(InstructionHelper.ExtractDataFromSet(data, string.Format("M{0:D2}_{1}", sfp.SlotPosition, "PC")));
                sfp.Tx_Power = double.Parse(InstructionHelper.ExtractDataFromSet(data, string.Format("M{0:D2}_{1}", sfp.SlotPosition, "TXP")));
                sfp.Rx_Power = double.Parse(InstructionHelper.ExtractDataFromSet(data, string.Format("M{0:D2}_{1}", sfp.SlotPosition, "RXP")));
                sfp.Module_Wave = double.Parse(InstructionHelper.ExtractDataFromSet(data, string.Format("M{0:D2}_{1}", sfp.SlotPosition, "W")));
                sfp.Transmission_Distance = double.Parse(InstructionHelper.ExtractDataFromSet(data, string.Format("M{0:D2}_{1}", sfp.SlotPosition, "TD")));
                sfp.Transmission_Rate = double.Parse(InstructionHelper.ExtractDataFromSet(data, string.Format("M{0:D2}_{1}", sfp.SlotPosition, "R")));
                sfp.Module_Temperature = double.Parse(InstructionHelper.ExtractDataFromSet(data, string.Format("M{0:D2}_{1}", sfp.SlotPosition, "T")));
                sfp.Tx_Power_State = int.Parse(InstructionHelper.ExtractDataFromSet(data, string.Format("M{0:D2}_{1}", sfp.SlotPosition, "TXPA")));
                sfp.Rx_Power_State = int.Parse(InstructionHelper.ExtractDataFromSet(data, string.Format("M{0:D2}_{1}", sfp.SlotPosition, "RXPA")));
                sfp.Module_Temperature_State = int.Parse(InstructionHelper.ExtractDataFromSet(data, string.Format("M{0:D2}_{1}", sfp.SlotPosition, "TA")));
                sfp.Type = "SFP+";
                sfp.Alarm = "";
            }
        }
    }
}
