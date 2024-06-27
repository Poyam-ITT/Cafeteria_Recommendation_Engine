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
            int bytesRead;

            while ((bytesRead = _stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received: " + message);
            }
        }

        public void Disconnect()
        {
            _stream.Close();
            _client.Close();
        }
    }
}
