using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlsunView.CommService
{
    /// <summary>
    /// 业务卡通信父类
    /// </summary>
    public class CardCommService : IDisposable
    {
        /// <summary>
        /// TCP客户端通信服务
        /// </summary>
        public TcpClientService Client { get; set; }
        /// <summary>
        /// 卡编号
        /// </summary>
        public int CardNumber { get; set; }

        /// <summary>
        /// 业务卡基本信息
        /// </summary>
        /// <returns></returns>
        public virtual string GetCardBasicInfo()
        {
            string cmd = string.Format("<C{0}_[B_?]>", CardNumber.ToString("00"));
            return Client.SendRecv(cmd);
        }

        /// <summary>
        /// 查询业务卡的数据信息
        /// </summary>
        /// <returns></returns>
        public virtual string GetCardDataInfo()
        {
            string cmd = string.Format("<C{0}_[STA_?]>", CardNumber.ToString("00"));
            return Client.SendRecv(cmd);
        }

        public void Dispose()
        {
            Client.Close();
        }
    }
}
