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
                                  where a.AIConfirm.Value == false
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
    }
}