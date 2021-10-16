using System;
using SimpleTcp;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client_2021_1015();
            client.Send("msg message1");
            Console.ReadLine();
        }
    }
}
