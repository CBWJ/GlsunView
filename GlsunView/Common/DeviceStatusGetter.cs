using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GlsunView.Domain;
using GlsunView.Models;
using GlsunView.CommService;

namespace GlsunView.Common
{
    public static class DeviceStatusGetter
    {
        static Dictionary<int, List<TopoNodeStatus>> _dicSubnetDeviceStatusSet;
        static Dictionary<string, string> _dicColor = new Dictionary<string, string>();
        static object _lockObj = new object();
        static DeviceStatusGetter()
        {
            string[,] colorTable = new string[6, 2] { { "CRITICAL", "#FF0000" },
                                                            { "MAJOR", "#FFA500" },
                                                            { "MINOR", "#FFFF00" },
                                                            { "WARN", "#00BFFF" },
                                                            { "NORMAL", "#00FF00" },
                                                            { "OFFLINE", "#CCCCCC" } };
            for(int i = 0; i < 6; ++i)
            {
                _dicColor.Add(colorTable[i, 0], colorTable[i, 1]);
            }
            _dicSubnetDeviceStatusSet = new Dictionary<int, List<TopoNodeStatus>>();
        }
        /// <summary>
        /// 获取某个设备的状态
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static TopoNodeStatus GetDeviceStatus(Device d)
        {
            TopoNodeStatus status = new TopoNodeStatus();
            status.ID = d.ID;
            if (d != null)
            {
                if (TcpClientDetectTool.IsOnline(d.DAddress, d.DPort.Value))
                {
                    //在线的则取当前告警状态
                    //status.Status = "NORMAL";
                    IEnumerable<string> alarms = null;
                    using(var ctx = new GlsunViewEntities())
                    {
                        alarms = (from a in ctx.AlarmInformation
                                  where a.AIConfirm.Value == false && a.DID == d.ID
                                  group a by a.AILevel into r
                                  select r.Key).ToList();
                    }
                    if (alarms.Contains("CRITICAL"))
                    {
                        status.Status = "CRITICAL";
                    }
                    else if (alarms.Contains("MAJOR"))
                    {
                        status.Status = "MAJOR";
                    }
                    else if (alarms.Contains("MINOR"))
                    {
                        status.Status = "MINOR";
                    }
                    else if (alarms.Contains("WARN"))
                    {
                        status.Status = "WARN";
                    }
                    else
                    {
                        status.Status = "NORMAL";
                    }
                }
                else
                {
                    status.Status = "OFFLINE";
                }
                status.BackgroundColor = _dicColor[status.Status];
                
            }
            return status;
        }
        //获取某个子网的状态
        public static TopoNodeStatus GetSubnetStatus(Subnet net)
        {
            TopoNodeStatus status = new TopoNodeStatus();
            status.ID = net.ID;
            status.Status = "OFFLINE";
            //确保该子网状态下的设备都取到状态
            IEnumerable<Device> devices = null;
            using (var ctx = new GlsunViewEntities())
            {
                devices = ctx.Device.Where(d => d.SID == net.ID).ToList();
            }
            _dicSubnetDeviceStatusSet.Clear();
            foreach (var d in devices)
            {
                SetStatusToSubnet(net.ID, GetDeviceStatus(d));
            }
            if (_dicSubnetDeviceStatusSet.ContainsKey(net.ID))
            {
                var list = _dicSubnetDeviceStatusSet[net.ID];
                var alarms = (from s in list
                              group s by s.Status into r
                              select r.Key).ToList();
                if (alarms.Contains("CRITICAL"))
                {
                    status.Status = "CRITICAL";
                }
                else if (alarms.Contains("MAJOR"))
                {
                    status.Status = "MAJOR";
                }
                else if (alarms.Contains("MINOR"))
                {
                    status.Status = "MINOR";
                }
                else if (alarms.Contains("WARN"))
                {
                    status.Status = "WARN";
                }
                else if(alarms.Contains("NORMAL"))
                {
                    status.Status = "NORMAL";
                }
            }
            status.BackgroundColor = _dicColor[status.Status];
            return status;
        }
        /// <summary>
        /// 设置设备状态到子网状态集合
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="status"></param>
        static void SetStatusToSubnet(int sid, TopoNodeStatus status)
        {
            if (_dicSubnetDeviceStatusSet.ContainsKey(sid))
            {//存在该子网
                var list = _dicSubnetDeviceStatusSet[sid];
                var oldStatus = (from s in list
                              where s.ID == status.ID
                              select s).FirstOrDefault();
                //存在该设备则更新
                if(oldStatus != null)
                {
                    oldStatus.Status = status.Status;
                    oldStatus.BackgroundColor = status.BackgroundColor;
                }
                else
                {
                    list.Add(status);
                }
            }
            else
            {//不存在该子网的状态数据
                List<TopoNodeStatus> list = new List<TopoNodeStatus>();
                list.Add(status);
                _dicSubnetDeviceStatusSet.Add(sid, list);
            }
        }
    }
}