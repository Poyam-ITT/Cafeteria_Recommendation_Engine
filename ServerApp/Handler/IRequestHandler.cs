using System.Net.Sockets;

namespace ServerApp.Handler
{
    public interface IRequestHandler
    {
        string HandleRequest(NetworkStream stream, string request, string role, int userId);
        void SendMessage(NetworkStream stream, string message);
    }
}
