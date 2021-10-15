using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp
{
    public class Test_Server_2021_1015
    {
        private ITcp m_tcp = null;
        public void Test()
        {
            m_tcp = new Tcp_NET5_Fixed_Syn();
            m_tcp.OnConnected += M_tcp_OnConnected;
            m_tcp.Listen(9000);
        }

        private void M_tcp_OnConnected(IConnection obj)
        {
            // 为新创建的连接绑定连接断开的回调
            obj.OnDisConnected += Obj_OnDisConnected;
            // 为新创建的连接绑定数据帧接收完成的回调
            obj.OnRcved += Obj_OnRcved;
        }

        private void Obj_OnRcved(byte[] obj)
        {
            throw new NotImplementedException();
        }

        private void Obj_OnDisConnected()
        {
            throw new NotImplementedException();
        }
    }
}
