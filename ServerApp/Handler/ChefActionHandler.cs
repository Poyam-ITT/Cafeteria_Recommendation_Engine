﻿using Microsoft.Extensions.DependencyInjection;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using RecommendationEngine.Services;
using System.Net.Sockets;
using System.Text;

namespace ServerApp.Handler
{
    public class ChefActionHandler : BaseActionHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public ChefActionHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string HandleChefActions(NetworkStream stream, string choice, int userId)
        {
            var chefService = _serviceProvider.GetService<IChefService>();
            var menuService = _serviceProvider.GetService<IMenuService>();
            var message = "";
            try
            {
                switch (choice)
                {
                    case "1":
                        return HandleRollOutItemsOperation(stream, chefService, userId);
                    case "2":
                        return HandleReportOperation(chefService);
                    case "3":
                        return HandleDiscardMenuItemList(stream, userId);
                    case "4":
                        return HandleViewFeedbackOperation();
                    case "5":
                        return HandleViewMenuItemsOperation(menuService);
                    case "6":
                        return "Logging out chef actions.\n";
                    default:
                        message = "Invalid choice.\n";
                        Console.WriteLine(message);
                        break;
                }
            }
            catch (FormatException ex)
            {
                message = $"Format Exception occurred: {ex.Message}\n";
            }
            catch (Exception ex)
            {
                message = $"An error occurred: {ex.Message}\n";
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

    }
}
