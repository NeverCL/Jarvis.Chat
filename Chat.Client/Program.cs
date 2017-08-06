using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // Linux iptables -A INPUT -p tcp --dport  8080-j ACCEPT
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8090));
            // client.Connect(new IPEndPoint(IPAddress.Parse("101.200.138.237"), 8090));
            // client.Connect(new DnsEndPoint("ja.youlijinfu.com", 8090));
            var clientWapper = new SocketClient(client);
            clientWapper.receiveEvent += data =>
            {
                System.Console.WriteLine($"Server {DateTime.Now}:{Encoding.UTF8.GetString(data)}");
            };
            System.Console.WriteLine("Begin Client");
            while (true)
            {
                clientWapper.Send(Console.ReadLine());
            }
        }
    }

    class SocketClient
    {
        private Socket clientSocket;
        // public string IP { get; set; }
        // public int Port { get; set; }
        // public SocketClient(string ip, string port)
        // {
        //     IP = ip
        // }

        public SocketClient(Socket client)
        {
            clientSocket = client;
            Task.Run(() =>
            {
                var data = new byte[1024];
                try
                {
                    var rst = clientSocket.Receive(data);
                    receiveEvent(data);
                }
                catch (SocketException)
                {
                    System.Console.WriteLine("Server 断开");
                }
            });
        }

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
