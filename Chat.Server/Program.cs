using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket server = new Socket(SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080));
            server.Listen(10);
            Task.Run(() =>
            {
                while (true)
                {
                    var client = server.Accept();
                    System.Console.WriteLine("Client 请求：" + DateTime.Now);
                    client.Send(Encoding.UTF8.GetBytes("Hello Server"));
                }
            });
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
