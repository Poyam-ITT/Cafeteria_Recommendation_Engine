using System.Net.Sockets;

namespace ServerApp.Handler
{
    public interface IClientHandler
    {
        void HandleClient(TcpClient client);
    }

}
