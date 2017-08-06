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
            server.Bind(new IPEndPoint(IPAddress.Any, 8090));
            server.Listen(10);
            Task.Run(() =>
            {
                while (true)
                {
                    var clientWapper = new SocketClient(server.Accept());
                    clientWapper.receiveEvent += data =>
                    {
                        System.Console.WriteLine("收到消息:" + Encoding.UTF8.GetString(data));
                    };
                    clientWapper.Send("Hello Server");
                }
            });
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
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
