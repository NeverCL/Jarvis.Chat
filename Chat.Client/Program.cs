using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
            var buffer = new byte[1024];
            var data = client.Receive(buffer);
            System.Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
        }
    }
}
