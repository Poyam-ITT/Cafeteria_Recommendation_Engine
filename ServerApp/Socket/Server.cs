using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using MySqlX.XDevAPI;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using RecommendationEngine.Services;

namespace RecommendationEngine.Sockets
{
    public class Server
    {
        private TcpListener _listener;
        private readonly int _port;
        private readonly IServiceProvider _serviceProvider;

        public Server(int port, IServiceProvider serviceProvider)
        {
            _port = port;
            _serviceProvider = serviceProvider;
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            Console.WriteLine($"Server started...");

            while (true)
            {
                var client = _listener.AcceptTcpClient();
                var thread = new Thread(HandleClient);
                thread.Start(client);
            }
        }

        private void HandleClient(object obj)
        {
            var client = (TcpClient)obj;
            var stream = client.GetStream();
            var buffer = new byte[1024];
            int bytesRead;

            // Read Employee ID
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            var employeeId = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

            // Read Name
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            var name = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

            var authService = _serviceProvider.GetService<IAuthService>();

            if (authService.Authenticate(employeeId, name, out string role))
            {
                var message = $"Welcome {name}! You are logged in as {role}.\n";

                if (role == "Admin")
                {
                    message += "Admin Actions:\nPress 1 to add a menu item\nPress 2 to update a menu item\nPress 3 to delete a menu item\nPress 4 to view menu\nPress 5 to Logout\n";
                }
                else if (role == "Chef")
                {
                    message += "Chef Actions:\nPress 1 to roll out items for tomorrow\nPress 2 to generate monthly feedback report\nPress 3 to Logout\n";
                }
                else if (role == "Employee")
                {
                    message += "Employee Actions:\nPress 1 to choose a menu item\nPress 2 to give feedback on a menu item\nPress 3 to Logout\n";
                }
                else
                {
                    message += "Invalid role.";
                }

                SendMessage(stream, message);

                while (true)
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var choice = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    if (role == "Admin")
                    {
                        message = HandleAdminActions(stream, choice);
                    }
                    else if (role == "Chef")
                    {
                        message = HandleChefActions(stream, choice);
                    }
                    else if (role == "Employee")
                    {
                        message = HandleEmployeeActions(stream, choice);
                    }

                    SendMessage(stream, message);

                    if(choice == "5" || choice == "3")
                    {
                        break;
                    }
                }
            }
            else
            {
                var message = "Authentication failed.";
                SendMessage(stream, message);
            }

            client.Close();
        }

        private void SendMessage(NetworkStream stream, string message)
        {
            var responseData = Encoding.ASCII.GetBytes(message);
            stream.Write(responseData, 0, responseData.Length);
        }

        private string HandleAdminActions(NetworkStream stream, string choice)
        {
            var menuService = _serviceProvider.GetService<IMenuService>();
            var buffer = new byte[1024];
            int bytesRead;
            var message = "";

            switch (choice)
            {
                case "1":
                    message = "Enter menu item name:\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var itemName = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    message = "Enter menu item price:\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var itemPrice = decimal.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Enter menu item availability status (true/false):\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var itemStatus = bool.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    menuService.AddMenuItem(itemName, itemPrice, itemStatus);
                    message = "Menu item added.\n";
                    Console.WriteLine(message);
                    break;
                case "2":
                    message = "Enter menu item ID:\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var updateId = int.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Enter new menu item name:\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var newName = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    message = "Enter new menu item price:\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var newPrice = decimal.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Enter new menu item availability status (true/false):\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var newStatus = bool.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    menuService.UpdateMenuItem(updateId, newName, newPrice, newStatus);
                    message = "Menu item updated.\n";
                    Console.WriteLine(message);
                    break;
                case "3":
                    message = "Enter menu item ID to delete:\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var deleteId = int.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    menuService.DeleteMenuItem(deleteId);
                    message = "Menu item deleted.\n";
                    Console.WriteLine(message);
                    break;
                case "4":
                    var menuItems = menuService.ViewMenuItems();
                    if (menuItems.Count == 0)
                    {
                        message = "No menu items found.\n";
                    }
                    else
                    {
                        message = "Menu Items:\n";
                        foreach (var item in menuItems)
                        {
                            message += $"ID: {item.Id}, Name: {item.Name}, Price: {item.Price}, Available: {item.AvailabilityStatus}\n";
                        }
                    }
                    Console.WriteLine(message);
                    break;
                case "5":
                    return "Logging out admin actions.\n";
                default:
                    message = "Invalid choice.\n";
                    break;
            }

            return message;
        }

        private string HandleChefActions(NetworkStream stream, string choice)
        {
            var chefService = _serviceProvider.GetService<IChefService>();
            var buffer = new byte[1024];
            int bytesRead;
            var message = "";

            switch (choice)
            {
                case "1":
                    message = "Enter menu type to roll out (Breakfast, Lunch, Dinner):\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var menuTypeStr = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    if (!Enum.TryParse<MenuType>(menuTypeStr, true, out MenuType menuType))
                    {
                        message = "Invalid menu type.\n";
                        break;
                    }

                    message = "Enter number of items to roll out:\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var itemCount = int.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    var items = new List<MenuItem>();

                    for (int i = 0; i < itemCount; i++)
                    {
                        message = $"Enter name for item {i + 1}:\n";
                        SendMessage(stream, message);

                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        var itemName = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                        message = "Enter price:\n";
                        SendMessage(stream, message);

                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        var itemPrice = decimal.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                        items.Add(new MenuItem
                        {
                            Name = itemName,
                            Price = itemPrice,
                            AvailabilityStatus = true,
                            MenuType = menuType
                        });
                    }

                    foreach (var item in items)
                    {
                        chefService.RollOutItems(menuType, itemCount);
                    }

                    message = "Items rolled out.\n";
                    break;
                    Console.WriteLine(message);
                case "2":
                    var report = chefService.GenerateMonthlyFeedbackReport();
                    message = $"Monthly Feedback Report:\n{report}\n";
                    Console.WriteLine(message);
                    break;
                case "3":
                    return "Logging out chef actions.\n";
                default:
                    message = "Invalid choice.\n";
                    Console.WriteLine(message);
                    break;
            }

            return message;
        }

        private string HandleEmployeeActions(NetworkStream stream, string choice)
        {
            var employeeService = _serviceProvider.GetService<IEmployeeService>();
            var menuService = _serviceProvider.GetService<IMenuService>();
            var buffer = new byte[1024];
            int bytesRead;
            var message = "";

            switch (choice)
            {
                case "1":
                    message = "Enter menu type to view (Breakfast, Lunch, Dinner):\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var menuTypeStr = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    if (Enum.TryParse<MenuType>(menuTypeStr, true, out MenuType menuType))
                    {
                        var rolledOutItems = employeeService.ViewRolledOutItems(menuType);
                        if (rolledOutItems.Count == 0)
                        {
                            message = "No rolled out items found.\n";
                        }
                        else
                        {
                            message = "Rolled Out Items:\n";
                            foreach (var item in rolledOutItems)
                            {
                                message += $"ID: {item.MenuItemId}, Name: {item.MenuItem.Name}, Price: {item.MenuItem.Price}, " +
                                           $"Recommendation: {item.RecommendationScore}, Rating: {item.AverageRating}, Sentiment: {item.SentimentScore}\n";
                            }
                        }
                    }
                    else
                    {
                        message = "Invalid menu type.\n";
                    }
                    Console.WriteLine(message);
                    break;
                case "2":
                    var menuItems = menuService.ViewMenuItems();
                    if (menuItems.Count == 0)
                    {
                        message = "No menu items found.\n";
                    }
                    else
                    {
                        message = "Menu Items:\n";
                        foreach (var item in menuItems)
                        {
                            message += $"ID: {item.Id}, Name: {item.Name}, Price: {item.Price}, Available: {item.AvailabilityStatus}\n";
                        }
                    }
                    SendMessage(stream, message);

                    message = "Enter menu item ID to give feedback on:\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var feedbackItemId = int.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Enter rating (1-5):\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var rating = int.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Enter comment:\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var comment = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    employeeService.GiveFeedback(feedbackItemId, rating, comment);
                    message = "Feedback submitted.\n";
                    Console.WriteLine(message);
                    break;
                case "3":
                    return "Logging out employee actions.\n";
                default:
                    message = "Invalid choice.\n";
                    Console.WriteLine(message);
                    break;
            }

            return message;
        }

    }
}
