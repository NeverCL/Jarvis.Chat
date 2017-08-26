using System;
using System.Collections.Generic;
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
            // 1. 开启服务器R端口监听，接受远程客户端（一般为本地内网）连接
            // 2. 当被请求的时候，转发消息传输给远程客户端，同时等待客户端返回
            // 3. 远程客户端接收请求，做出响应返回。
            // 4. 服务器端将响应返回给请求


            var list = new List<SocketClient>();
            Socket server = new Socket(SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, 8090));
            server.Listen(10);
            Task.Run(() =>
            {
                while (true)
                {
                    var client = server.Accept();
                    var clientWapper = new SocketClient(client);
                    clientWapper.receiveEvent += data =>
                    {
                        System.Console.WriteLine("收到消息:" + Encoding.UTF8.GetString(data));
                    };
                    clientWapper.Send("Hello Server");
                    list.Add(clientWapper);
                }
            });
            Console.WriteLine("Hello World!");
            while (true)
            {
                var str = Console.ReadLine();
                list.ForEach(item => item.Send(str));
            }
        }
    }

    class SocketWapper
    {

    }

    // SocketWapper
    class SocketClient
    {
        // Accept创建的socket
        private Socket clientSocket;
        public string IP { get; set; }
        public int Port { get; set; }
        public SocketClient(Socket client)
        {
            System.Console.WriteLine($"{DateTime.Now} {(client.RemoteEndPoint as IPEndPoint).Address}");
            var ipAddress = (client.RemoteEndPoint as IPEndPoint);
            IP = ipAddress.Address.ToString();
            Port = ipAddress.Port;
            clientSocket = client;
            Task.Run(() =>
            {
                while (true)
                {
                    var data = new byte[1024];
                    try
                    {
                        var rst = clientSocket.Receive(data);
                        receiveEvent(data);
                    }
                    catch (SocketException)
                    {
                        System.Console.WriteLine("Client 离开");
                        throw;
                    }
                }
            });
        }

        // 发消息给客户端
        public void Send(byte[] data)
        {
            clientSocket.Send(data);
        }

        public void Send(string message)
        {
            clientSocket.Send(Encoding.UTF8.GetBytes(message));
        }

        // 接收消息的事件
        public event Action<byte[]> receiveEvent;
    }
}
