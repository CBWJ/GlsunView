using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace GlsunView.CommService
{
    public class TcpClientService : IDisposable
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public int RecvTimeOut { get; set; }
        public string DataPattern { get; set; }
        /// <summary>
        /// 服务正忙
        /// </summary>
        public bool IsBusy { get; set; }
        public DateTime LastUseTime { get; set; }
        /// <summary>
        /// 客户端
        /// </summary>
        private TcpClient _tcpClient;
        /// <summary>
        /// 线程同步事件
        /// </summary>
        private ManualResetEvent _eventRecieve;
        /// <summary>
        /// 接受数据
        /// </summary>
        private StringBuilder _sbRecv;
        /// <summary>
        /// 是否停止接收
        /// </summary>
        private bool _bStop;
        private object _lock = new object();
        /// <summary>
        /// 连接同步事件
        /// </summary>
        private ManualResetEvent _eventConnect;
        /// <summary>
        /// 连接超时
        /// </summary>
        public int ConnectTimeout { get; set; }
        /// <summary>
        /// 异步连接是否成功
        /// </summary>
        private bool _isConnectionSuccessful;
        public TcpClientService()
        {
            IsBusy = false;
            _tcpClient = new TcpClient();
            _eventRecieve = new ManualResetEvent(false);
            _sbRecv = new StringBuilder();
            _eventConnect = new ManualResetEvent(false);
        }

        public TcpClientService(string ip, int port, int timeout = 100, string pattern = @"^<\S+>$")
        {
            IP = ip;
            Port = port;
            RecvTimeOut = timeout;
            DataPattern = pattern;
            IsBusy = false;
            _tcpClient = new TcpClient();
            _eventRecieve = new ManualResetEvent(false);
            _sbRecv = new StringBuilder();
            _eventConnect = new ManualResetEvent(false);
        }

        public void Start()
        {
            _bStop = false;
            //开启一个线程来接受服务器发来的数据
            Task.Factory.StartNew(()=> {
                while (!_bStop)
                {
                    try
                    {
                        if (_tcpClient.Connected)
                        {
                            var recvStream = _tcpClient.GetStream();
                            if (recvStream.CanRead)
                            {
                                byte[] buffer = new byte[2 * 1024];
                                int recvSize = recvStream.Read(buffer, 0, buffer.Length);
                                if (recvSize == 0)
                                    continue;
                                _sbRecv.Append(Encoding.ASCII.GetString(buffer, 0, recvSize));
                                if (Regex.IsMatch(_sbRecv.ToString(), DataPattern))
                                {
                                    //收到正确数据，发送信号，让调用线程不再等待
                                    _eventRecieve.Set();
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        //发生异常则通知线程不再等待
                        _sbRecv.Clear();
                        _sbRecv.Append(ex.Message);
                        _eventRecieve.Set();
                        break;
                    }
                }
            });
        }
        public void Stop()
        {
            _bStop = true;
        }

        public void Connect()
        {
            _tcpClient.Connect(IP, Port);
            Start();
        }
        /// <summary>
        /// 超时连接
        /// </summary>
        /// <returns>true:超时时间内连接成功</returns>
        public bool ConnectWithTimeout()
        {
            try
            {
                //事件信号重置
                _eventConnect.Reset();
                //异步连接
                _tcpClient.BeginConnect(IP, Port, new AsyncCallback(requestCallback), _tcpClient);
                //超时时间内收到信号
                if (_eventConnect.WaitOne(ConnectTimeout))
                {
                    if (_isConnectionSuccessful)
                    {
                        //连接成功，开启接收线程
                        Start();
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    _tcpClient.Close();
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public void Close()
        {
            Stop();
            _tcpClient.Close();
        }

        public string SendRecv(string cmd)
        {
            //为什么加锁？这个服务是共享的，多个线程可能同时共享一个，
            //此时对会该对象的字段进行读写，保证在同一时刻，只有个线程使用该方法
            lock (_lock)
            {
                string ret = "";
                if (_tcpClient.Connected)
                {
                    var sendStream = _tcpClient.GetStream();
                    var buffer = Encoding.ASCII.GetBytes(cmd);
                    if (sendStream.CanWrite)
                    {
                        _eventRecieve.Reset();//重置信号
                        _sbRecv.Clear();
                        sendStream.Write(buffer, 0, buffer.Length);
                        //超时内等待信号
                        if (_eventRecieve.WaitOne(RecvTimeOut))
                        {
                            //正常处理
                            ret = _sbRecv.ToString();
                        }
                        else
                        {
                            //超时处理
                            ret = "读取超时：" + _sbRecv.ToString();
                        }
                    }
                }
                return ret;
            }
        }

        /// <summary>
        /// 同步发送异步接收
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        //public string SendRecv(string cmd)
        //{
        //    string ret = "";
        //    if (_tcpClient.Connected)
        //    {
        //        var sendStream = _tcpClient.GetStream();
        //        var buffer = Encoding.ASCII.GetBytes(cmd);
        //        if (sendStream.CanWrite)
        //        {
        //            _eventRecieve.Reset();//重置信号
        //            _sbRecv.Clear();
        //            sendStream.Write(buffer, 0, buffer.Length);
        //            sendStream.Flush();
        //            byte[] bufferRecv = new byte[2 * 1024];
        //            sendStream.BeginRead(bufferRecv, 0, bufferRecv.Length, new AsyncCallback(ReadCallback),
        //                new AsynState { Buffer = bufferRecv, Stream = sendStream });
        //            //超时内等待信号
        //            if (_eventRecieve.WaitOne(RecvTimeOut))
        //            {
        //                //正常处理
        //                ret = _sbRecv.ToString();
        //            }
        //            else
        //            {
        //                //超时处理
        //                ret = "读取超时";
        //            }
        //        }
        //        else
        //        {
        //            //Console.WriteLine("can not write...");
        //        }
        //    }
        //    else
        //    {
        //        //Console.WriteLine("not connected...");
        //    }
        //    return ret;
        //}
        /// <summary>
        /// 异步读取
        /// </summary>
        /// <param name="iar"></param>
        public void ReadCallback(IAsyncResult iar)
        {
            var state = (AsynState)iar.AsyncState;
            int readSize = state.Stream.EndRead(iar);
            var data = Encoding.ASCII.GetString(state.Buffer, 0, readSize);
            _sbRecv.Append(data);
            if (Regex.IsMatch(_sbRecv.ToString(), DataPattern))
            {
                //收到正确数据，发送信号，让调用线程不再等待
                _eventRecieve.Set();
            }
            else
            {
                state.Stream.BeginRead(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(ReadCallback),
                    new AsynState { Buffer = state.Buffer, Stream = state.Stream });
            }
        }
        /// <summary>
        /// 异步连接回调
        /// </summary>
        /// <param name="ar"></param>
        public void requestCallback(IAsyncResult ar)
        {
            try
            {
                _isConnectionSuccessful = false;
                var tcpClient = ar.AsyncState as TcpClient;
                //套接字不为空
                if (tcpClient.Client != null)
                {
                    tcpClient.EndConnect(ar);
                    _isConnectionSuccessful = true;
                }
            }
            catch
            {
                _isConnectionSuccessful = false;
            }
            finally
            {
                _eventConnect.Set();
            }
        }
        public void Dispose()
        {
            Close();
        }
    }
}
