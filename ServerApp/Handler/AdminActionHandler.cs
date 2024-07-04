using Microsoft.Extensions.DependencyInjection;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using System.Net.Sockets;
using System.Text;

namespace ServerApp.Handler
{
    internal class AdminActionHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public AdminActionHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string HandleAdminActions(NetworkStream stream, string choice, int userId)
        {
            var menuService = _serviceProvider.GetService<IMenuService>();
            var notificationService = _serviceProvider.GetService<INotificationService>();
            var message = "";

            switch (choice)
            {
                case "1":
                    return HandleAddMenuItemOperation(stream, menuService, notificationService, userId);
                case "2":
                    return HandleUpdateMenuItemOperation(stream, menuService, notificationService, userId);
                case "3":
                    return HandleDeleteMenuItemOperation(stream, menuService);
                case "4":
                    return HandleViewMenuItemsOperation(menuService);
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

        private string HandleAddMenuItemOperation(NetworkStream stream, IMenuService menuService, INotificationService notificationService, int userId)
        {
            var buffer = new byte[1024];
            int bytesRead;
            var message = "";
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
                return message;
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

            return message;
        }
        
        private string HandleUpdateMenuItemOperation(NetworkStream stream, IMenuService menuService, INotificationService notificationService, int userId)
        {
            var buffer = new byte[1024];
            int bytesRead;
            var message = "";
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

            if (!Enum.TryParse<MenuType>(newType, true, out MenuType menuType))
            {
                message = "Invalid menu type.\n";
                return message;
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
            var notification = new Notification
            {
                UserId = userId,
                Message = $"Item Updated: {updateItem.Name}",
                Type = NotificationType.AvailabilityStatus.ToString(),
                Date = DateTime.Now
            };
            notificationService.SendNotification(notification);

            message = "Menu item updated.\n";
            Console.WriteLine(message);
            return message;
        }
        
        private string HandleDeleteMenuItemOperation(NetworkStream stream, IMenuService menuService)
        {
            var buffer = new byte[1024];
            int bytesRead;
            var message = "";
            message = "Enter menu item ID to delete:\n";
            SendMessage(stream, message);

            bytesRead = stream.Read(buffer, 0, buffer.Length);
            var deleteId = int.Parse(Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim());

            menuService.DeleteMenuItem(deleteId);
            message = "Menu item deleted.\n";
            Console.WriteLine(message);
            return message;
        }

        private string HandleViewMenuItemsOperation(IMenuService menuService)
        {
            var message = "";
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

        private void SendMessage(NetworkStream stream, string message)
        {
            var responseData = Encoding.ASCII.GetBytes(message);
            stream.Write(responseData, 0, responseData.Length);
        }
    }
}
