using System;
using System.Net.Sockets;
using System.Text;

namespace RecommendationEngine.Sockets
{
    public class Client
    {
        private readonly int _port;
        private TcpClient _client;
        private NetworkStream _stream;

        public Client(int port)
        {
            _port = port;
        }

        public void Connect()
        {
            _client = new TcpClient("127.0.0.1", _port);
            _stream = _client.GetStream();
            Console.WriteLine($"Connected to server...");
        }

        public void SendMessage(string message)
        {
            var data = Encoding.ASCII.GetBytes(message);
            _stream.Write(data, 0, data.Length);
        }

        public void ReceiveMessages()
        {
            var buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Disconnected from server.");
                        break;
                    }
                    var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine(message);

                    if (message.Contains("Logging out") || message.Contains("DISCONNECT"))
                    {
                        Console.WriteLine("Logged out. Disconnecting...");
                        _client.Close();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    break;
                }
            }
        }

        public void Disconnect()
        {
            _stream.Close();
            _client.Close();
            Console.WriteLine("Disconnected from server.");
        }
    }
}
