using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp
{
    public class Client_2021_1015
    {
        private ITcp m_tcp;
        public void Send(string msg)
        {
            m_tcp = new Tcp_NET5_Fixed_Syn();
            IConnection conn= m_tcp.Connect("127.0.0.1", 9000);
            conn.OnDisConnected += Conn_OnDisConnected;
            conn.OnRcved += Conn_OnRcved;
            conn.Recv();
            conn.Send(Encoding.UTF8.GetBytes(msg));
        }

        private void Conn_OnRcved(byte[] obj)
        {
            //throw new NotImplementedException();
        }

        private void Conn_OnDisConnected()
        {
            //throw new NotImplementedException();
        }
    }
}
