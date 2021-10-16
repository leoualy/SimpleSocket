using System;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server_2021_1015();
            server.Start();
            Console.ReadLine();
        }
    }
}
