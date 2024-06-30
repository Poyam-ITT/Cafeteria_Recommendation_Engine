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

            if (authService.Authenticate(employeeId, name, out string role, out int userId))
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
                    message += "Employee Actions:\nPress 1 to view rolled out items\nPress 2 to give feedback on a menu item\nPress 3 to View Notifications\nPress 4 to Logout\n";
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
                        message = HandleAdminActions(stream, choice, userId);
                    }
                    else if (role == "Chef")
                    {
                        message = HandleChefActions(stream, choice, userId);
                    }
                    else if (role == "Employee")
                    {
                        message = HandleEmployeeActions(stream, choice, userId);
                    }

                    if ((role == "Admin" && choice == "5") || (role == "Chef" && choice == "3") || (role == "Employee" && choice == "4"))
                    {
                        SendMessage(stream, "Logging out. Goodbye!");
                        break;
                    }
                    SendMessage(stream, message);
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

        private string HandleAdminActions(NetworkStream stream, string choice, int userId)
        {
            var menuService = _serviceProvider.GetService<IMenuService>();
            var notificationService = _serviceProvider.GetService<INotificationService>();
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

                    message = "Enter menu type (Breakfast, Lunch, Dinner):\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var itemType = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    if (!Enum.TryParse<MenuType>(itemType, true, out MenuType menuType))
                    {
                        message = "Invalid menu type.\n";
                        break;
                    }

                    menuService.AddMenuItem(itemName, itemPrice, itemStatus, menuType);
                    // Send notification
                    var notification = new Notification
                    {
                        UserId = userId,
                        Message = $"New Item: {itemName}",
                        Type = NotificationType.NewItem.ToString(),
                        Date = DateTime.Now
                    };
                    notificationService.SendNotification(notification);
                    message = "Menu item added.\n";
                    Console.WriteLine(message);
                    break;
                case "2":
                    message = "Enter menu item ID to update:\n";
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

                    message = "Enter new menu type (Breakfast, Lunch, Dinner):\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var newType = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    if (!Enum.TryParse<MenuType>(newType, true, out menuType))
                    {
                        message = "Invalid menu type.\n";
                        break;
                    }

                    menuService.UpdateMenuItem(updateId, newName, newPrice, newStatus, menuType);
                    //send notification
                    notification = new Notification
                    {
                        UserId = userId,
                        Message = $"Item Availability Updated: {newName}",
                        Type = NotificationType.AvailabilityStatus.ToString(),
                        Date = DateTime.Now
                    };
                    notificationService.SendNotification(notification);
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
                            message += $"ID: {item.Id}, Name: {item.Name}, Price: {item.Price}, Available: {item.AvailabilityStatus}, Type: {item.MenuType}\n";
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

        private string HandleChefActions(NetworkStream stream, string choice, int userId)
        {
            var chefService = _serviceProvider.GetService<IChefService>();
            var notificationService = _serviceProvider.GetService<INotificationService>();
            var buffer = new byte[1024];
            int bytesRead;
            var message = "";

            switch (choice)
            {
                case "1":
                    message = "Enter menu type to roll out (Breakfast, Lunch, Dinner):\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var typeOfMenu = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    if (!Enum.TryParse<MenuType>(typeOfMenu, true, out MenuType menuType))
                    {
                        message = "Invalid menu type.\n";
                        SendMessage(stream, message);
                        break;
                    }

                    message = "Enter recommended item list size:\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var listSize = int.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    var recommendedItems = chefService.GetFoodItemForNextDay(menuType, listSize);
                    if (recommendedItems.Count == 0)
                    {
                        message = "No recommended items found for the selected menu type.\n";
                        SendMessage(stream, message);
                        break;
                    }

                    message = "Recommended Items:\n";
                    foreach (var item in recommendedItems)
                    {
                        message += $"ID: {item.MenuItem.Id}, Name: {item.MenuItem.Name}, Price: {item.MenuItem.Price}, " +
                                   $"Recommendation: {item.Score}, Rating: {item.AverageRating}, Sentiment: {item.PositiveSentiments}\n";
                    }
                    SendMessage(stream, message);

                    message = "Enter number of items to roll out:\n";
                    SendMessage(stream, message);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var itemCount = int.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    var itemsToRollOut = new List<MenuItem>();
                    for (int i = 0; i < itemCount; i++)
                    {
                        message = $"Enter ID for item {i + 1} to roll out:\n";
                        SendMessage(stream, message);

                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        var itemId = int.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                        var selectedItem = recommendedItems.FirstOrDefault(r => r.MenuItem.Id == itemId)?.MenuItem;
                        if (selectedItem != null)
                        {
                            itemsToRollOut.Add(selectedItem);
                        }
                        else
                        {
                            message = $"Item with ID {itemId} not found in recommendations.\n";
                            SendMessage(stream, message);
                        }
                    }

                    chefService.RollOutItems(menuType, itemsToRollOut);
                    // Send notification
                    var notification = new Notification
                    {
                        UserId = userId,
                        Message = "Recommendation: New menu items rolled out for tomorrow!",
                        Type = NotificationType.Recommendation.ToString(),
                        Date = DateTime.Now
                    };
                    notificationService.SendNotification(notification);
                    message = "Items rolled out.\n";
                    Console.WriteLine(message);
                    break;
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

        private string HandleEmployeeActions(NetworkStream stream, string choice, int userId)
        {
            var employeeService = _serviceProvider.GetService<IEmployeeService>();
            var notificationService = _serviceProvider.GetService<INotificationService>();
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
                            message += $"ID: {item.Id}, Name: {item.Name}, Price: {item.Price}, Available: {item.AvailabilityStatus}, Type: {item.MenuType}\n";
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

                    employeeService.GiveFeedback(feedbackItemId, rating, comment, userId);
                    message = "Feedback submitted.\n";
                    Console.WriteLine(message);
                    break;
                case "3":
                    message = "Viewing notifications...\n";
                    var notifications = notificationService.GetNotifications(1);
                    foreach (var notification in notifications)
                    {
                        message += $"ID: {notification.Id}, Message: {notification.Message}, Type: {notification.Type}, Date: {notification.Date}\n";
                    }
                    break;
                case "4":
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
