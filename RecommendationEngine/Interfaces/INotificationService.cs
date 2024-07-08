using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface INotificationService
    {
        void SendNotification(Notification notification);
        public IEnumerable<Notification> GetNotifications();
    }
}