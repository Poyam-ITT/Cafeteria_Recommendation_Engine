using Microsoft.Extensions.DependencyInjection;
using RecommendationEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Handler
{
    public class BaseActionHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public BaseActionHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string HandleViewMenuItemsOperation(IMenuService menuService)
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

        public string HandleDiscardMenuItemList(NetworkStream stream)
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

        public string HandleViewFeedbackOperation()
        {
            var feedbackService = _serviceProvider.GetService<IFeedbackService>();
            var message = "";
            var feedback = feedbackService.GetAllFeedbacks();
            if (feedback == null)
            {
                message = "No feedback found.\n";
            }
            else
            {
                message = "Feedback:\n";
                foreach (var item in feedback)
                {
                    message += $"ID: {item.Id}, UserId: {item.UserId}, MenuItemId: {item.MenuItemId}, Rating: {item.Rating}, Comment: {item.Comment}, FeedbackDate: {item.FeedbackDate}\n";
                }
            }
            Console.WriteLine(message);
            return message;
        }

        public void SendMessage(NetworkStream stream, string message)
        {
            var responseData = Encoding.ASCII.GetBytes(message);
            stream.Write(responseData, 0, responseData.Length);
        }
    }
}
