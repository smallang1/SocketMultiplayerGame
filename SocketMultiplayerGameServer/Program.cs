using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using SocketMultiplayerGameServer.Servers;

namespace SocketMultiplayerGameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server sever = new Server(6666); //创建服务端对象，监听端口
            Console.WriteLine("服务器启动......");
            Console.Read();
        }
    }
}
