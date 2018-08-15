using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GlsunView.CommService
{
    /// <summary>
    /// EDFA卡通信服务
    /// author: 代码小菜
    /// date: 2018-5-2
    /// </summary>
    public class EDFACommService : CardCommService
    {
        public EDFACommService(string ip, int port, int cardNum)
        {
            Client = new TcpClientService(ip, port);
            Client.Connect();
            CardNumber = cardNum;
        }
        public EDFACommService(TcpClientService client, int cardNum)
        {
            Client = client;
            CardNumber = cardNum;
        }
        #region 查询
        /// <summary>
        /// 获取工作模式
        /// </summary>
        /// <returns></returns>
        public string GetWorkMode()
        {
            string cmd = string.Format("<C{0}_[M_?]>", CardNumber.ToString("00"));
            return Client.SendRecv(cmd);
        }
        /// <summary>
        /// 获取当前增益
        /// </summary>
        /// <returns></returns>
        public string GetGain()
        {
            string cmd = string.Format("<C02_[CPGV_?]>", CardNumber.ToString("00"));
            return Client.SendRecv(cmd);
        } 
        #endregion

        #region 设置
        /// <summary>
        /// 设置工作模式
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [Description("设置工作模式")]
        public bool SetWorkMode(int mode)
        {
            string cmd = string.Format("<C{0}_[M_{1}]>", CardNumber.ToString("00"), mode);
            string result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[M_SETOK]>", CardNumber.ToString("00")))
                return true;
            else
                return false;
        }
        /// <summary>
        /// 设置增益
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置增益")]
        public bool SetGainSetting(double value)
        {
            var arrNum = value.ToString("F2").Split('.');
            string cmd = string.Format("<C{0}_[PGV_+{1}.{2}]>", CardNumber.ToString("00"), arrNum[0].PadLeft(2, '0'), arrNum[1]);
            string result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[PGV_SETOK]>", CardNumber.ToString("00")))
                return true;
            else
                return false;
        } 
        #endregion
    }
}
