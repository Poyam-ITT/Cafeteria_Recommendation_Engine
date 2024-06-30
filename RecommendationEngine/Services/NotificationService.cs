using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;

namespace RecommendationEngine.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public void SendNotification(Notification notification)
        {
            _notificationRepository.Save(notification);
        }

        public IEnumerable<Notification> GetNotifications(int userId)
        {
            return _notificationRepository.FindByUserId(userId);
        }
    }
}
