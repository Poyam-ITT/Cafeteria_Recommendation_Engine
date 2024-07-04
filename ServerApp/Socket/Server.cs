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
            _listener.Start();

            while (true)
            {
                var client = _listener.AcceptTcpClient();
                var thread = new Thread(() => _clientHandler.HandleClient(client));
                thread.Start();
            }
        }
    }
}
