using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace GlsunView.CommService
{
    public class AsynState
    {
        public byte[] Buffer { get; set; }
        public NetworkStream Stream { get; set; }
    }
}
