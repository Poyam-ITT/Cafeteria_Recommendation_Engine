using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface INotificationRepository
    {
        void Save(Notification notification);
        Notification FindById(int id);
        IEnumerable<Notification> FindByUserId(int userId);
    }
}
