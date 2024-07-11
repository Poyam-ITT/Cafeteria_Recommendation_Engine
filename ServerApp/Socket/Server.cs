using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Handler;

namespace ServerApp.Socket
{
    public class Server
    {
        private readonly TcpListener _listener;
        private readonly IServiceProvider _serviceProvider;
        private readonly IClientHandler _clientHandler;

        public Server(int port, IServiceProvider serviceProvider)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _serviceProvider = serviceProvider;
            _clientHandler = serviceProvider.GetService<IClientHandler>();
        }

        public void Start()
        {
            try
            {
                _listener.Start();

                while (true)
                {
                    var client = _listener.AcceptTcpClient();
                    var thread = new Thread(() => _clientHandler.HandleClient(client));
                    thread.Start();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket error when accepting a client: {ex.Message}");
            }
            catch (ThreadStateException ex)
            {
                Console.WriteLine($"Error when starting a client handler thread: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}
