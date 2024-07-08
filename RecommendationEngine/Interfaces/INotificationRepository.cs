using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface INotificationRepository
    {
        void Save(Notification notification);
        IEnumerable<Notification> FindByUserId();
    }
}
