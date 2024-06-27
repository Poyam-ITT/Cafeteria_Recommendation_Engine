/*using MySqlX.XDevAPI;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using System;

namespace RecommendationEngine.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly Client _client;

        public NotificationService(INotificationRepository notificationRepository, int port)
        {
            _notificationRepository = notificationRepository;
            _client = new Client(port);
            _client.Connect();
        }

        public void SendRecommendationNotification(string message)
        {
            var notification = new Notification
            {
                UserId = 1, // Example userId, replace with actual logic
                Message = "Recommendation: " + message,
                Type = "Recommendation",
                Date = DateTime.Now
            };
            _notificationRepository.Save(notification);
            _client.SendNotification(notification.Message);
        }

        public void SendNewItemNotification(string message)
        {
            var notification = new Notification
            {
                UserId = 1, // Example userId, replace with actual logic
                Message = "New Item: " + message,
                Type = "NewItem",
                Date = DateTime.Now
            };
            _notificationRepository.Save(notification);
            _client.SendNotification(notification.Message);
        }

        public void SendAvailabilityStatusNotification(string message)
        {
            var notification = new Notification
            {
                UserId = 1, // Example userId, replace with actual logic
                Message = "Availability Status: " + message,
                Type = "AvailabilityStatus",
                Date = DateTime.Now
            };
            _notificationRepository.Save(notification);
            _client.SendNotification(notification.Message);
        }

        public void SendNotification(string message)
        {
            var notification = new Notification
            {
                UserId = 1, // Example userId, replace with actual logic
                Message = message,
                Type = "General",
                Date = DateTime.Now
            };
            _notificationRepository.Save(notification);
            _client.SendNotification(notification.Message);
        }
    }
}
*/