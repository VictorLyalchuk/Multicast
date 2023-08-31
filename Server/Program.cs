using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main(string[] args)
    {
        try
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress broadcast = IPAddress.Parse("192.168.1.255");

            while (true)
            {

                Console.Write($"Input message = ");
                string message = Console.ReadLine();
                byte[] sendbuf = Encoding.Unicode.GetBytes(message);
                IPEndPoint ep = new IPEndPoint(broadcast, 11000);

                s.SendTo(sendbuf, ep);

                Console.WriteLine("Message sent to the broadcast address");
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
}