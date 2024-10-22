using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerApp
{
    class Server
    {
        const int port = 4040;
        const int maxClients = 5;  
        UdpClient udpServer;
        List<IPEndPoint> clients = new List<IPEndPoint>();

        public void Start()
        {
            udpServer = new UdpClient(port);
            Console.WriteLine("Сервер запущений...");

            while (true)
            {
                IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] buffer = udpServer.Receive(ref clientEndpoint);
                string message = Encoding.UTF8.GetString(buffer);

                if (message.StartsWith("JOIN:"))
                {
                    if (clients.Count < maxClients)
                    {
                        if (!clients.Contains(clientEndpoint))
                        {
                            clients.Add(clientEndpoint);
                            Console.WriteLine($"{clientEndpoint} під'єднався.");
                            BroadcastMessage($"[Сервер]: {message.Substring(5)} під'єднався до чату.", clientEndpoint);
                        }
                    }
                    else
                    {
                        SendMessage("Сервер перевантажений. Максимальна кількість клієнтів досягнута.", clientEndpoint);
                    }
                }
                else if (message.StartsWith("LEAVE:"))
                {
                    clients.Remove(clientEndpoint);
                    Console.WriteLine($"{clientEndpoint} покинув чат.");
                    BroadcastMessage($"[Сервер]: {message.Substring(6)} покинув чат.", clientEndpoint);
                }
                else if (message.StartsWith("PRIVATE:"))
                {
                    string[] parts = message.Split(':');
                    if (parts.Length > 2)
                    {
                        string recipient = parts[1];
                        string privateMessage = parts[2];
                        SendPrivateMessage(recipient, privateMessage, clientEndpoint);
                    }
                }
                else
                {
                    BroadcastMessage(message, clientEndpoint);
                }
            }
        }

        void BroadcastMessage(string message, IPEndPoint sender)
        {
            string timestampedMessage = $"[{DateTime.Now.ToShortTimeString()}] {message}";
            byte[] buffer = Encoding.UTF8.GetBytes(timestampedMessage);

            foreach (var client in clients)
            {
                if (!client.Equals(sender))
                {
                    udpServer.Send(buffer, buffer.Length, client);
                }
            }
        }

        void SendMessage(string message, IPEndPoint recipient)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            udpServer.Send(buffer, buffer.Length, recipient);
        }

        void SendPrivateMessage(string recipientName, string message, IPEndPoint sender)
        {
            string senderAddress = sender.ToString();
            IPEndPoint recipientEndpoint = clients.Find(client => client.ToString().Contains(recipientName));
            if (recipientEndpoint != null)
            {
                SendMessage($"[Приватне повідомлення від {senderAddress}]: {message}", recipientEndpoint);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
        }
    }
}
