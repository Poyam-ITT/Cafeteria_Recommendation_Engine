using System.Net.Sockets;
using System.Text;

namespace ServerApp.Handler
{
    public class RequestHandler : IRequestHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private AdminActionHandler _adminActionHandler;
        private ChefActionHandler _chefActionHandler;
        private EmployeeActionHandler _employeeActionHandler;

        public RequestHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _adminActionHandler = new AdminActionHandler(serviceProvider);
            _chefActionHandler = new ChefActionHandler(serviceProvider);
            _employeeActionHandler = new EmployeeActionHandler(serviceProvider);
        }

        public string HandleRequest(NetworkStream stream, string request, string role, int userId)
        {
            return role switch
            {
                "Admin" => _adminActionHandler.HandleAdminActions(stream, request, userId),
                "Chef" => _chefActionHandler.HandleChefActions(stream, request, userId),
                "Employee" => _employeeActionHandler.HandleEmployeeActions(stream, request, userId),
                _ => "Invalid role."
            };
        }

        public void SendMessage(NetworkStream stream, string message)
        {
            var responseData = Encoding.ASCII.GetBytes(message);
            stream.Write(responseData, 0, responseData.Length);
        }
    }
}
