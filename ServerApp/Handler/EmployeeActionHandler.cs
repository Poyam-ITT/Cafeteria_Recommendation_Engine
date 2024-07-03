using Microsoft.Extensions.DependencyInjection;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using System.Net.Sockets;
using System.Text;

namespace ServerApp.Handler
{
    public class EmployeeActionHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public EmployeeActionHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string HandleEmployeeActions(NetworkStream stream, string choice, int userId)
        {
            var employeeService = _serviceProvider.GetService<IEmployeeService>();
            var notificationService = _serviceProvider.GetService<INotificationService>();
            var message = "";

            switch (choice)
            {
                case "1":
                    return HandleViewRollOutItemsOperation(stream, employeeService);
                case "2":
                    return HandleFeedbackOperation(stream, employeeService, userId);
                case "3":
                    return HandleViewNotificationOperation(notificationService);
                case "4":
                    return HandleUpdateProfileOperation(stream, userId);
                case "5":
                    return "Logging out employee actions.\n";
                default:
                    message = "Invalid choice.\n";
                    Console.WriteLine(message);
                    break;
            }

            return message;
        }
        private string HandleViewRollOutItemsOperation(NetworkStream stream,IEmployeeService employeeService)
        {
            var message = "";
            var buffer = new byte[1024];
            int bytesRead;

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
            return message;
        }

        private string HandleFeedbackOperation(NetworkStream stream, IEmployeeService employeeService, int userId)
        {
            var message = "";
            var buffer = new byte[1024];
            int bytesRead;
            var feedbackService = _serviceProvider.GetService<IFeedbackService>();
            var menuService = _serviceProvider.GetService<IMenuService>();

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
            return message;
        }

        private string HandleViewNotificationOperation(INotificationService notificationService)
        {
            var message = "Viewing notifications...\n";
            var notifications = notificationService.GetNotifications(1);
            foreach (var notification in notifications)
            {
                message += $"ID: {notification.Id}, Message: {notification.Message}, Type: {notification.Type}, Date: {notification.Date}\n";
            }
            Console.WriteLine(message);
            return message;
        }

        private string HandleUpdateProfileOperation(NetworkStream stream, int userId)
        {
            var message = "";
            var buffer = new byte[1024];
            int bytesRead;
            var profileService = _serviceProvider.GetService<IEmployeeProfileService>();

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
            return message;
        }

        private void SendMessage(NetworkStream stream, string message)
        {
            var responseData = Encoding.ASCII.GetBytes(message);
            stream.Write(responseData, 0, responseData.Length);
        }
    }
}
