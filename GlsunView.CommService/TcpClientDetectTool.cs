using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlsunView.CommService
{
    public static class TcpClientDetectTool
    {
        public static bool IsOnline(string ip, int port)
        {
            bool result = false;
            TcpClientService _tcp = new TcpClientService(ip, port);
            _tcp.ConnectTimeout = 500;
            if (_tcp.ConnectWithTimeout())
            {
                result = true;
            }
            _tcp.Close();
            return result;
        }
    }
}
