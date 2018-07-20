using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;

namespace GlsunView.CommService
{
    public class TcpClientServicePool
    {
        private static System.Timers.Timer _timer;
        private static Dictionary<string, List<TcpClientService>> _dicServices;
        private static object _locker;
        static TcpClientServicePool()
        {
            _timer = new System.Timers.Timer();
            //1分钟处理一次
            _timer.Interval = 30000;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
            //服务
            _dicServices = new Dictionary<string, List<TcpClientService>>();

            //同步
            _locker = new object();
        }

        private static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var item in _dicServices)
                {
                    var list = _dicServices[item.Key];
                    var temp = (from t in list
                                select t).ToList();
                    foreach (var tcp in temp)
                    {
                        var span = DateTime.Now - tcp.LastUseTime;
                        //10秒内没使用过释放
                        if (tcp.IsBusy == false && span.Seconds >= 30)
                        {
                            tcp.Close();
                            list.Remove(tcp);
                            Console.WriteLine("释放tcp");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 外部接口:根据IP与端口获取一个TCP连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static TcpClientService GetService(string ip, int port)
        {
            TcpClientService ret = null;
            string key = string.Format("{0}:{1}", ip, port);
            //加锁确保只有一个线程操作服务集合
            lock (_locker)
            {
                if (_dicServices.ContainsKey(key))
                {
                    //客户端已存在
                    var list = _dicServices[key];
                    foreach (var tcp in list)
                    {
                        //取空闲
                        if (tcp.IsBusy == false)
                        {
                            ret = tcp;
                            break;
                        }
                    }
                    //无空闲
                    if (ret == null)
                    {
                        var tcp = GetServiceSimple(ip, port);
                        if (tcp != null)
                        {
                            list.Add(tcp);
                            ret = tcp;
                        }
                    }
                }
                else
                {
                    //首次请求
                    List<TcpClientService> list = new List<TcpClientService>();
                    var tcp = GetServiceSimple(ip, port);
                    if (tcp != null)
                    {
                        list.Add(tcp);
                        ret = tcp;
                        _dicServices.Add(key, list);
                    }
                }
            }
            if(ret != null)
            {
                ret.IsBusy = true;
                ret.LastUseTime = DateTime.Now;
            }
            return ret;
        }

        private static TcpClientService GetServiceSimple(string ip, int port)
        {
            TcpClientService tcp = new TcpClientService(ip, port);
            tcp.ConnectTimeout = 500;
            if (tcp.ConnectWithTimeout())
            {
                return tcp;   
            }
            return null;
        }
    }
}
