using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleTcp;

namespace TestServer
{
    public class Server_2021_1015
    {
        private ITcp m_tcp = null;
        public void Start()
        {
            m_tcp = new Tcp_NET5_Fixed_Syn();
            m_tcp.OnConnected += M_tcp_OnConnected;
            m_tcp.Listen(9000);
        }

        private void M_tcp_OnConnected(IConnection obj)
        {
            Console.WriteLine("Connected");
            // 为新创建的连接绑定连接断开的回调
            obj.OnDisConnected += Obj_OnDisConnected;
            // 为新创建的连接绑定数据帧接收完成的回调
            obj.OnRcved += Obj_OnRcved;
            obj.Recv();
        }

        private void Obj_OnRcved(byte[] obj)
        {
            Console.WriteLine(Encoding.UTF8.GetString(obj));
        }

        private void Obj_OnDisConnected()
        {
            //throw new NotImplementedException();
        }
    }
}
