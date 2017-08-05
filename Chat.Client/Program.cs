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
            Socket client = new Socket(SocketType.Stream, ProtocolType.Tcp);
            client.Connect(new DnsEndPoint("ja.youlijinfu.com",8080));
            // client.Connect(new DnsEndPoint("localhost",8080));
            // client.Connect(new IPEndPoint());
            var buffer = new byte[1024];
            var data = client.Receive(buffer);

            // iptables -A INPUT -p tcp --dport  8080-j ACCEPT
            System.Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
        }
    }
}
