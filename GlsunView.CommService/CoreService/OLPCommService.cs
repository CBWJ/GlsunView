using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GlsunView.CommService
{
    public class OLPCommService : CardCommService
    {
        public OLPCommService(string ip, int port, int cardNum)
        {
            Client = new TcpClientService(ip, port);
            Client.Connect();
            CardNumber = cardNum;
        }
        public OLPCommService(TcpClientService client, int cardNum)
        {
            Client = client;
            CardNumber = cardNum;
        }

        #region 设置
        /// <summary>
        /// R1通道波长设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置R1通道波长")]
        public bool SetR1Wave(int value)
        {
            string cmd = string.Format("<C{0}_[R1_W_{1}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[R1_W_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// R2通道波长设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置R2通道波长")]
        public bool SetR2Wave(int value)
        {
            string cmd = string.Format("<C{0}_[R2_W_{1}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[R2_W_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Tx通道波长设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置Tx通道波长")]
        public bool SetTxWave(int value)
        {
            string cmd = string.Format("<C{0}_[TX_W_{1}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[TX_W_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// R1切换光功率阈值设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置R1切换光功率阈值")]
        public bool SetR1SwitchingPower(double value)
        {
            string cmd = string.Format("<C{0}_[R1_SP_{1:F3}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[R1_SP_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// R2切换光功率阈值设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置R2切换光功率阈值")]
        public bool SetR2SwitchingPower(double value)
        {
            string cmd = string.Format("<C{0}_[R2_SP_{1:F3}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[R2_SP_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// R1告警光功率阈值设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置R1告警光功率阈值")]
        public bool SetR1AlarmPower(double value)
        {
            string cmd = string.Format("<C{0}_[R1_AP_{1:F3}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[R1_AP_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// R2告警光功率阈值设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置R2告警光功率阈值")]
        public bool SetR2AlarmPower(double value)
        {
            string cmd = string.Format("<C{0}_[R1_AP_{1:F3}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[R1_AP_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Tx告警光功率阈值设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置Tx告警光功率阈值")]
        public bool SetTXAlarmPower(double value)
        {
            string cmd = string.Format("<C{0}_[TX_AP_{1:F3}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[TX_AP_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 手动/自动模式设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置手动/自动模式")]
        public bool SetWorkMode(int value)
        {
            string cmd = string.Format("<C{0}_[M_{1}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[M_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 回切方式的设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置回切方式的")]
        public bool SetBackMode(int value)
        {
            string cmd = string.Format("<C{0}_[ACC_{1}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[ACC_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 设置自动回切延时时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置自动回切延时时间")]
        public bool SetAutoBackDelay(int value)
        {
            string cmd = string.Format("<C{0}_[Q_{1}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[Q_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 自动切换延时
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置自动切换延时")]
        public bool SetSwitchDelay(int value)
        {
            string cmd = string.Format("<C{0}_[Y_{1}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[Y_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 主路/备路设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置主路/备路")]
        public bool SetManualSwitchChannel(int value)
        {
            string cmd = string.Format("<C{0}_[S_{1}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[S_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 手动模式自动返回自动模式延时
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置手动返回自动延时")]
        public bool SetManualBacktoAutoDelay(int value)
        {
            string cmd = string.Format("<C{0}_[R_{1}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[R_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 当前工作模式掉电保存设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("设置当前工作模式掉电保存")]
        public bool SetPowerOffKeepWorkMode(int value)
        {
            string cmd = string.Format("<C{0}_[WMH_{1}]>", CardNumber.ToString("00"), value);
            var result = Client.SendRecv(cmd);
            if (result == string.Format("<C{0}_[WMH_SETOK]>", CardNumber.ToString("00")))
            {
                return true;
            }
            else
                return false;
        }

        #endregion
    }
}
