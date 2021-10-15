using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SimpleTcp
{
    #region TCP
    public class Tcp_NET5_Fixed_Syn : BaseTcp
    {
        protected override IConnection CreateConnection(Socket s)
        {
            return new ConnectionSimple(s);
        }
    }

    public abstract class BaseTcp : SocketCommon, ITcp
    {
        public IConnection Connect(string ip, int port)
        {
            var s = CreateSocket_IPV4();
            s.Connect(IPAddress.Parse(ip), port);
            return CreateConnection(s);
        }

        public bool Listen(int port, string ip = null)
        {
            m_running = true;
            m_socketServer = CreateSocket_IPV4();
            m_socketServer.Bind(CreateEndPoint(port, null));
            m_socketServer.Listen(10);
            m_conns = new List<IConnection>();
            Task.Factory.StartNew(() =>
            {
                while (m_running)
                {
                    var s = m_socketServer.Accept();
                    var conn = CreateConnection(s);
                    m_conns.Add(conn);
                    OnConnected?.Invoke(conn);
                }
            });
            return false;
        }
        public bool StopListen()
        {
            m_running = false;
            return true;
        }

        protected abstract IConnection CreateConnection(Socket s);

        private bool m_running = false;
        private Socket m_socketServer;
        private List<IConnection> m_conns;

        public event Action<IConnection> OnConnected;
    }

    public abstract class SocketCommon
    {
        protected Socket CreateSocket_IPV4()
        {
            return new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
        }
        protected EndPoint CreateEndPoint(int port, string ip = null)
        {
            return new IPEndPoint(ip == null ? IPAddress.Any : IPAddress.Parse(ip), port);
        }
    }

    public interface ITcp
    {
        event Action<IConnection> OnConnected;
        
        IConnection Connect(string ip,int port);
        bool Listen(int port,string ip=null);
        bool StopListen();
    }
    #endregion

    #region Connection
    public class ConnectionSimple : BaseConnection
    {
        /*
         * 帧格式
         * 头长度4字节：存储内容长度
         * 
         */
        private readonly ushort HEAD_LEN = 4;
        public ConnectionSimple(Socket s) : base(s) { }
        public override void Recv()
        {
            Task.Factory.StartNew(() =>
            {
                var len = 0;
                var rcvLen = 0;
                var _buf_head = new byte[HEAD_LEN];
                
                while (true)
                {
                    rcvLen = 0;
                    // 数组清零
                    Array.Clear(_buf_head, 0, _buf_head.Length);
                    while (rcvLen < HEAD_LEN)
                    {
                        len = TryRecv(_buf_head, rcvLen, HEAD_LEN - rcvLen);
                        if (len <= 0) return;
                        rcvLen += len;
                    }

                    // 内容长度
                    var cntLen = BitConverter.ToInt32(_buf_head, 0);
                    var _buf = new byte[cntLen];
                    rcvLen = 0;
                    while (rcvLen < cntLen)
                    {
                        len = TryRecv(_buf, rcvLen, cntLen - rcvLen);
                        if (len <= 0) return;
                        rcvLen += len;
                    }
                    // 帧接收完成后的回调
                    Rcved(_buf);
                }
            });
        }
        public override void Send(byte[] buf)
        {
            var _buf_head = BitConverter.GetBytes(buf.Length);
            var _buf = new byte[HEAD_LEN + buf.Length];
            _buf_head.Take(4).ToArray().CopyTo(_buf, 0);
            buf.CopyTo(_buf, HEAD_LEN);
            Task.Run(() =>
            {
                m_socket.Send(_buf);
            });
        }

        private int TryRecv(byte[] buf,int offset,int size)
        {
            try
            {
                var len = m_socket.Receive(buf, offset, size, SocketFlags.None);
                if (len > 0) return len;
                // 连接断开的处理
                DisConnected();
                return 0;
            }
            catch(Exception ex)
            {
                DisConnected();
                return -1;
            }
        }
    }
    public abstract class BaseConnection : IConnection
    {
        public BaseConnection(Socket s)
        {
            m_socket = s;
        }
        public abstract void Recv();
        public abstract void Send(byte[] buf);

        protected void DisConnected()
        {
            OnDisConnected?.Invoke();
        }
        protected void Rcved(byte[] buf)
        {
            OnRcved?.Invoke(buf);
        }

        protected Socket m_socket;
        protected Action<byte[]> m_cb;

        public event Action OnDisConnected;
        public event Action<byte[]> OnRcved;
    }
    public interface IConnection
    {
        event Action OnDisConnected;
        event Action<byte[]> OnRcved;
        void Recv();
        void Send(byte[] buf);
    }
    #endregion
}
