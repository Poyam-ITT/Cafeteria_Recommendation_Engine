﻿using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using RecommendationEngine.Interfaces;

namespace ServerApp.Handler
{

    public class ClientHandler : IClientHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IRequestHandler _requestHandler;

        public ClientHandler(IServiceProvider serviceProvider, IRequestHandler requestHandler)
        {
            _serviceProvider = serviceProvider;
            _requestHandler = requestHandler;
        }

        public void HandleClient(TcpClient client)
        {
            using (client)
            {
                var stream = client.GetStream();
                var buffer = new byte[1024];
                int bytesRead;

                try
                {
                    // Read Employee ID
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var employeeId = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    // Read Name
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var name = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    var authService = _serviceProvider.GetService<IAuthService>();

                    if (authService.Authenticate(employeeId, name, out string role, out int userId))
                    {
                        var welcomeMessage = $"Welcome {name}! You are logged in as {role}.\n" + GetRoleSpecificActions(role);
                        _requestHandler.SendMessage(stream, welcomeMessage);

                        while (true)
                        {
                            bytesRead = stream.Read(buffer, 0, buffer.Length);
                            var choice = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                            var responseMessage = _requestHandler.HandleRequest(stream, choice, role, userId);

                            if (IsLogoutRequest(role, choice))
                            {
                                _requestHandler.SendMessage(stream, "Logging out. Goodbye!");
                                break;
                            }

                            _requestHandler.SendMessage(stream, responseMessage);
                        }
                    }
                    else
                    {
                        _requestHandler.SendMessage(stream, "Authentication failed.");
                    }
                }
                catch (Exception ex)
                {
                    _requestHandler.SendMessage(stream, $"An error occurred: {ex.Message}");
                }
            }
        }

        private string GetRoleSpecificActions(string role)
        {
            return role switch
            {
                "Admin" => "Admin Actions:\nPress 1 to add a menu item\nPress 2 to update a menu item\nPress 3 to delete a menu item\nPress 4 to view menu\nPress 5 to View Discard Menu items\nPress 6 to Logout\n",
                "Chef" => "Chef Actions:\nPress 1 to roll out items for tomorrow\nPress 2 to generate monthly feedback report\nPress 3 to View Discard Menu Items\nPress 4 to View feedback\nPress 5 to Logout\n",
                "Employee" => "Employee Actions:\nPress 1 to view rolled out items\nPress 2 to give feedback on a menu item\nPress 3 to View Notifications\nPress 4 to Update Profile\nPress 5 to Logout\n",
                _ => "Invalid role."
            };
        }

        private bool IsLogoutRequest(string role, string choice)
        {
            return (role == "Admin" && choice == "6") ||
                   (role == "Chef" && choice == "5") ||
                   (role == "Employee" && choice == "5");
        }
    }
}
