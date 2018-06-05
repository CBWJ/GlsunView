using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlsunView.CommService;

namespace GlsunView.Models
{
    public class DeviceOverview 
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public string Type { get; set; }
        public string CardStatus { get; set; }
        public int Unit { get; set; }
        public int SlotCount { get; set; }
        public List<Slot> Slots { get; set; }
        public void RefreshStatus(NMUCommService service)
        {
            Type = InstructionHelper.ExtractData(service.GetDeviceModel(), "DTP");
            CardStatus = InstructionHelper.ExtractData(service.GetBusinessCardStatus(), "CS");
            if (Type.Length > 0)
            {
                Unit = Convert.ToInt32(Type.Substring(Type.Length - 1));
            }
            var arrStatus = CardStatus.Split('_');
            Slots = new List<Slot>();
            if (arrStatus.Length > 2)
            {
                if(Slots == null)
                {
                    Slots = new List<Slot>();
                }
                else
                {
                    Slots.Clear();
                }
                SlotCount = Convert.ToInt32(arrStatus[0]);
                int index = 2;
                int slot = 1;
                foreach(var c in arrStatus[1])
                {
                    var card = "NOCARD";
                    var cardCode = arrStatus[index++];
                    if(cardCode == "0101" || cardCode == "0102" || cardCode == "0103")
                    {
                        card = "OLP";
                    }
                    else if(cardCode == "0301")
                    {
                        card = "EDFA";
                    }
                    else if(cardCode == "0701")
                    {
                        card = "OEO";
                    }
                    Slots.Add(new Slot { SlotNumber = slot++, IsInsert = c == '1', CardType = card});
                }
            }
        }
    }
}
