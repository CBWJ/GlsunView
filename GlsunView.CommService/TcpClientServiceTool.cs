using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Timers;

namespace GlsunView.CommService
{
    public static class TcpClientServiceTool
    {
        private static Timer _timerServiceMaintain = null;
        private static int _timeout = 500;
        private static object _lock = new object();
        private static object _locker = new object();
        private static Dictionary<IPEndPoint, List<TcpClientService>> _dicServicePool = new Dictionary<IPEndPoint, List<TcpClientService>>();
        public static TcpClientService GetService(string ip, int port)
        {
            if (_timerServiceMaintain == null)
            {
                _timerServiceMaintain = new Timer();
                _timerServiceMaintain.Interval = 1000;
                _timerServiceMaintain.Elapsed += _timerServiceMaintain_Elapsed;
                _timerServiceMaintain.Enabled = true;
            }
            //为什么加锁，会对共享的_dicServicePool进行读写
            lock (_lock)
            {
                IPEndPoint point = new IPEndPoint(IPAddress.Parse(IPFix(ip)), port);
                TcpClientService service = null;
                if (_dicServicePool.ContainsKey(point))
                {
                    var services = _dicServicePool[point];
                    foreach (var e in services)
                    {
                        if (e.IsBusy == false)
                        {
                            service = e;
                        }
                    }
                    //未找到空闲
                    if (service == null)
                    {
                        service = new TcpClientService(IPFix(ip), port, _timeout);
                        service.Connect();
                        services.Add(service);
                        _dicServicePool[point] = services;
                    }
                }
                else
                {
                    List<TcpClientService> services = new List<TcpClientService>();
                    service = new TcpClientService(IPFix(ip), port, _timeout);
                    service.Connect();
                    services.Add(service);
                    _dicServicePool[point] = services;
                }
                service.IsBusy = true;
                service.LastUseTime = DateTime.Now;
                return service;
            }
        }
        /// <summary>
        /// 服务维护定时器线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void _timerServiceMaintain_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var pair in _dicServicePool)
            {
                foreach (var tcp in pair.Value)
                {
                    var span = DateTime.Now - tcp.LastUseTime;
                    //5分钟后清理空闲服务
                    if (span.Seconds > 300)
                    {
                        pair.Value.Remove(tcp);
                        tcp.Close();
                    }
                }
            }
        }

        public static void SetServiceFree(TcpClientService service)
        {
            service.IsBusy = false;
        }
        public static int GetServiceCount()
        {
            return _dicServicePool.Count;
        }
        public static Dictionary<IPEndPoint, List<TcpClientService>> GetServiceDictionary()
        {
            return _dicServicePool;
        }
        /// <summary>
        /// 去掉IP中的0前缀，否则IPAddress解析会当作八进制
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        static private string IPFix(string ip)
        {
            //return ip.Replace(".0", ".");
            return ip;
        }
    }
}
