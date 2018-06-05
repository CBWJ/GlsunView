using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlsunView.CommService
{
    public class NMUCommService : IDisposable
    {
        public TcpClientService Client { get; set; }
        public NMUCommService(string ip, int port)
        {
            Client = new TcpClientService(ip, port);
            Client.Connect();
        }
        public NMUCommService(TcpClientService client)
        {
            Client = client;
        }
        
        public void Dispose()
        {
            if(Client != null)
            {
                Client.Close();
            }
        }

        #region 查询
        /// <summary>
        /// 设备型号查询
        /// </summary>
        /// <returns></returns>
        public string GetDeviceModel()
        {
            return Client.SendRecv("<C00_[DTP_?]>");
        }
        /// <summary>
        /// 业务卡插卡状态查询
        /// </summary>
        /// <returns></returns>
        public string GetBusinessCardStatus()
        {
            return Client.SendRecv("<C00_[CS_?]>");
        }
        /// <summary>
        /// IP地址查询
        /// </summary>
        /// <returns></returns>
        public string GetIPAddress()
        {
            return Client.SendRecv("<C00_[IP_?]>");
        }
        /// <summary>
        /// Net Mask查询
        /// </summary>
        /// <returns></returns>
        public string GetNetMask()
        {
            return Client.SendRecv("<C00_[MSK_?]>");
        }
        /// <summary>
        /// Gate Way查询
        /// </summary>
        /// <returns></returns>
        public string GetGateway()
        {
            return Client.SendRecv("<C00_[GW_?]>");
        }
        /// <summary>
        /// MAC 地址查询
        /// </summary>
        /// <returns></returns>
        public string GetMACAddress()
        {
            return Client.SendRecv("<C00_[MAC_?]>");
        }
        /// <summary>
        /// 按键锁定查询
        /// </summary>
        /// <returns></returns>
        public string GetKeyLock()
        {
            return Client.SendRecv("<C00_[KEY_?]>");
        }
        /// <summary>
        /// 风扇控制开关查询
        /// </summary>
        /// <returns></returns>
        public string GetFanControlSwitch()
        {
            return Client.SendRecv("<C00_[FNC_?]>");
        }
        /// <summary>
        /// 风扇工作状态查询
        /// </summary>
        /// <returns></returns>
        public string GetFanWorkingStatus()
        {
            return Client.SendRecv("<C00_[FNS_?]>");
        }
        /// <summary>
        /// 电源工作状态查询
        /// </summary>
        /// <returns></returns>
        public string GetPowerWorkingStatus()
        {
            return Client.SendRecv("<C00_[PWR_?]>");
        }
        /// <summary>
        /// 设备软件版本号查询
        /// </summary>
        /// <returns></returns>
        public string GetDeviceSoftwareVersion()
        {
            return Client.SendRecv("<C00_[SV_?]>");
        }
        /// <summary>
        /// 设备硬件版本号查询
        /// </summary>
        /// <returns></returns>
        public string GetDeviceHardwareVersion()
        {
            return Client.SendRecv("<C00_[HV_?]>");
        }
        /// <summary>
        /// 设备生产序列号查询
        /// </summary>
        /// <returns></returns>
        public string GetDeviceProductionSerialNumber()
        {
            return Client.SendRecv("<C00_[SN_?]>");
        }
        /// <summary>
        /// 设备出厂日期查询
        /// </summary>
        /// <returns></returns>
        public string GetDeviceFactoryDate()
        {
            return Client.SendRecv("<C00_[MD_X]>");
        }
        #endregion

        #region 设置
        /// <summary>
        /// IP地址设置
        /// </summary>
        /// <param name="data">参数</param>
        /// <returns></returns>
        public bool SetIPAddress(string data)
        {
            var result = Client.SendRecv("<C00_[IP_" + data + "]>");
            if (result == "<C00_[IP_SETOK]>")
                return true;
            else
                return false;
        }
        /// <summary>
        /// Net Mask设置
        /// </summary>
        /// <param name="data">参数</param>
        /// <returns></returns>
        public bool SetNetMask(string data)
        {
            var result = Client.SendRecv("<C00_[MSK_" + data + "]>");
            if (result == "<C00_[MSK_SETOK]>")
                return true;
            else
                return false;
        }
        /// <summary>
        /// Gate Way设置
        /// </summary>
        /// <param name="data">参数</param>
        /// <returns></returns>
        public bool SetGateway(string data)
        {
            var result = Client.SendRecv("<C00_[GW_" + data + "]>");
            if (result == "<C00_[GW_SETOK]>")
                return true;
            else
                return false;
        }
        /// <summary>
        /// 按键锁定设置
        /// </summary>
        /// <param name="data">参数</param>
        /// <returns></returns>
        public bool SetKeyLock(string data)
        {
            var result = Client.SendRecv("<C00_[KEY_" + data + "]>");
            if (result == "<C00_[KEY_SETOK]>")
                return true;
            else
                return false;
        }
        /// <summary>
        /// 风扇控制开关设置
        /// </summary>
        /// <param name="data">参数</param>
        /// <returns></returns>
        public bool SetFanControlSwitch(string data)
        {
            var result = Client.SendRecv("<C00_[FNC_" + data + "]>");
            if (result == "<C00_[FNC_SETOK]>")
                return true;
            else
                return false;
        } 
        #endregion

    }
}
