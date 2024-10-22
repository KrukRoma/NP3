using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientChat
{
    class Client
    {
        const int port = 4040;
        UdpClient udpClient;
        IPEndPoint serverEndpoint;

        public void Start(string userName)
        {
            udpClient = new UdpClient();
            serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            SendMessage($"JOIN:{userName}");

            Task.Run(() => Listen());

            while (true)
            {
                string message = Console.ReadLine();

                if (message.StartsWith("/private"))
                {
                    string[] parts = message.Split(' ', 3);
                    if (parts.Length == 3)
                    {
                        string recipient = parts[1];
                        string privateMessage = parts[2];
                        SendMessage($"PRIVATE:{recipient}:{privateMessage}");
                    }
                }
                else if (message == "/exit")
                {
                    SendMessage($"LEAVE:{userName}");
                    break;
                }
                else
                {
                    SendMessage($"{userName}: {message}");
                }
            }
        }

        void SendMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            udpClient.Send(buffer, buffer.Length, serverEndpoint);
        }

        void Listen()
        {
            IPEndPoint server = null;
            while (true)
            {
                byte[] buffer = udpClient.Receive(ref server);
                string message = Encoding.UTF8.GetString(buffer);
                Console.WriteLine(message);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введіть ваше ім'я: ");
            string userName = Console.ReadLine();

            Client client = new Client();
            client.Start(userName);
        }
    }
}