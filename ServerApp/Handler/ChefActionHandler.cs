using Microsoft.Extensions.DependencyInjection;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using System.Net.Sockets;
using System.Text;

namespace ServerApp.Handler
{
    public class ChefActionHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public ChefActionHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string HandleChefActions(NetworkStream stream, string choice, int userId)
        {
            var chefService = _serviceProvider.GetService<IChefService>();
            var message = "";

            switch (choice)
            {
                case "1":
                    return HandleRollOutItemsOperation(stream, chefService, userId);
                case "2":
                    return HandleReportOperation(chefService);
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

        private string HandleRollOutItemsOperation(NetworkStream stream, IChefService chefService, int userId)
        {
            var recommendationEngine = _serviceProvider.GetService<IRecommendationEngine>();
            var notificationService = _serviceProvider.GetService<INotificationService>();
            var buffer = new byte[1024];
            int bytesRead;
            var message = "";

            message = "Enter menu type to roll out (Breakfast, Lunch, Dinner):\n";
            SendMessage(stream, message);
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            var typeOfMenu = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

            if (!Enum.TryParse<MenuType>(typeOfMenu, true, out MenuType menuType))
            {
                message = "Invalid menu type.\n";
                SendMessage(stream, message);
                return message;
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
                return message;
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
            return message;
        }
        
        private string HandleReportOperation(IChefService chefService)
        {
            var report = chefService.GenerateMonthlyFeedbackReport();
            var message = $"Monthly Feedback Report:\n{report}\n";
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
                    SendMessage(stream, "Enter the Id of the food item to get detailed feedback for:");
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
