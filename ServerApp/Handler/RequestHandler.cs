using Microsoft.Extensions.DependencyInjection;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using System.Net.Sockets;
using System.Text;

namespace ServerApp.Handler
{
    public class RequestHandler : IRequestHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string HandleRequest(NetworkStream stream, string request, string role, int userId)
        {
            return role switch
            {
                "Admin" => HandleAdminActions(stream, request, userId),
                "Chef" => HandleChefActions(stream, request, userId),
                "Employee" => HandleEmployeeActions(stream, request, userId),
                _ => "Invalid role."
            };
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
                    var menuItem = new MenuItem();

                    message = "Enter menu item name:\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    menuItem.Name = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    message = "Enter menu item price:\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    menuItem.Price = decimal.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Enter menu item availability status (true/false):\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    menuItem.AvailabilityStatus = bool.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Enter menu type (Breakfast, Lunch, Dinner):\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var itemType = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    if (!Enum.TryParse<MenuType>(itemType, true, out MenuType menuType))
                    {
                        message = "Invalid menu type.\n";
                        break;
                    }
                    menuItem.MenuType = menuType;

                    message = "Is the menu item vegetarian? (true/false):\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    menuItem.IsVegetarian = bool.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Is the menu item non-vegetarian? (true/false):\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    menuItem.IsNonVegetarian = bool.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Is the menu item eggetarian? (true/false):\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    menuItem.IsEggetarian = bool.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Enter spice level:\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    menuItem.SpiceLevel = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    menuService.AddMenuItem(menuItem);

                    // Send notification
                    var notification = new Notification
                    {
                        UserId = userId,
                        Message = $"New Item: {menuItem.Name}",
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

                    var updateItem = new MenuItem();

                    message = "Enter new menu item name:\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    updateItem.Name = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    message = "Enter new menu item price:\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    updateItem.Price = decimal.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Enter new menu item availability status (true/false):\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    updateItem.AvailabilityStatus = bool.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Enter new menu type (Breakfast, Lunch, Dinner):\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var newType = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    if (!Enum.TryParse<MenuType>(newType, true, out menuType))
                    {
                        message = "Invalid menu type.\n";
                        break;
                    }
                    updateItem.MenuType = menuType;

                    message = "Is the menu item vegetarian? (true/false):\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    updateItem.IsVegetarian = bool.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Is the menu item non-vegetarian? (true/false):\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    updateItem.IsEggetarian = bool.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

                    message = "Enter new spice level:\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    updateItem.SpiceLevel = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    menuService.UpdateMenuItem(updateId, updateItem);

                    // Send notification
                    notification = new Notification
                    {
                        UserId = userId,
                        Message = $"Item Updated: {updateItem.Name}",
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
                    return HandleDiscardMenuItemList(stream);
                case "6":
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
            var recommendationEngine = _serviceProvider.GetService<IRecommendationEngine>();
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

                    var recommendedItems = recommendationEngine.GetFoodItemForNextDay(menuType, listSize, userId);
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

                    chefService.RollOutItems(menuType, itemsToRollOut, userId);
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
                    return HandleDiscardMenuItemList(stream);
                case "4":
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
            var feedbackService = _serviceProvider.GetService<IFeedbackService>();
            var profileService = _serviceProvider.GetService<IEmployeeProfileService>();
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
                    feedbackService.MoveLowRatedItemsToDiscardedList();
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
                    Console.WriteLine(message);
                    break;
                case "4":
                    message = "Updating your profile...\n";
                    SendMessage(stream, message);

                    message = "Please select one (Vegetarian, Non Vegetarian, Eggetarian):\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var preferenceType = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    message = "Please select your spice level (High, Medium, Low):\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var spiceLevel = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    message = "What do you prefer most? (North Indian, South Indian, Other):\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var preferredCuisine = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    message = "Do you have a sweet tooth? (Yes, No):\n";
                    SendMessage(stream, message);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var sweetTooth = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim().ToLower() == "yes";

                    var profile = new EmployeeProfile
                    {
                        UserId = userId,
                        PreferenceType = preferenceType,
                        SpiceLevel = spiceLevel,
                        PreferredCuisine = preferredCuisine,
                        SweetTooth = sweetTooth
                    };

                    profileService.SaveProfile(profile);
                    message = "Profile updated successfully.\n";
                    Console.WriteLine(message);
                    break;
                case "5":
                    return "Logging out employee actions.\n";
                default:
                    message = "Invalid choice.\n";
                    Console.WriteLine(message);
                    break;
            }

            return message;
        }

        private string HandleDiscardMenuItemList(NetworkStream stream)
        {
            var menuService = _serviceProvider.GetService<IMenuService>();
            var discardMenuItems = menuService.GetDiscardMenuItems();

            if (discardMenuItems.Count == 0)
            {
                return "No items to discard.";
            }

            var message = "Discard Menu Item List:\n";
            foreach (var item in discardMenuItems)
            {
                message += $"- Id: {item.Id} || Name: {item.Name}\n";
            }

            message += "Options:\n1) Remove the Food Item from Menu List\n2) Get Detailed Feedback\n";
            SendMessage(stream, message);

            var buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            var choice = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

            switch (choice)
            {
                case "1":
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
                    SendMessage(stream, "Enter the id of the food item to remove:");
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var foodItemId = int.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());
                    menuService.DeleteMenuItem(foodItemId);
                    menuService.RemoveFromDiscardedMenuItems(foodItemId);
                    return $"Item Id:{foodItemId} is removed from the menu.";
                case "2":
                    SendMessage(stream, "Enter the name of the food item to get detailed feedback for:");
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    foodItemId = int.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());
                    return $"Detailed feedback added for {foodItemId}.";
                default:
                    return "Invalid choice.";
            }
        }

        public void SendMessage(NetworkStream stream, string message)
        {
            var responseData = Encoding.ASCII.GetBytes(message);
            stream.Write(responseData, 0, responseData.Length);
        }
    }
}
