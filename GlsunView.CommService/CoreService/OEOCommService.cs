using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GlsunView.CommService
{
    public class OEOCommService : CardCommService
    {
        public OEOCommService(string ip, int port, int cardNum)
        {
            Client = new TcpClientService(ip, port);
            Client.Connect();
            CardNumber = cardNum;
        }
        public OEOCommService(TcpClientService client, int cardNum)
        {
            Client = client;
            CardNumber = cardNum;
        }
        /// <summary>
        /// 设置模块工作模式
        /// </summary>
        /// <param name="module"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [Description("设置工作模式")]
        public bool SetWorkMode(int module, int mode)
        {
            var cmd = string.Format("<C{0:D2}_[M{1:D2}_M_{2}]>", CardNumber, module, mode);
            var result = Client.SendRecv(cmd);
            if(result == string.Format("<C{0:D2}_[M{1:D2}_M_{2}]>", CardNumber, module, "SETOK"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 设置光模块发光功率控制
        /// </summary>
        /// <param name="module"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [Description("设置发光功率控制")]
        public bool SetTxPowerControl(int module, int mode)
        {
            var cmd = string.Format("<C{0:D2}_[M{1:D2}_PC_{2}]>", CardNumber, module, mode);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0:D2}_[M{1:D2}_PC_{2}]>", CardNumber, module, "SETOK"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
