using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface IFeedbackRepository
    {
        void Save(Feedback feedback);
        Feedback FindById(int id);
        IEnumerable<Feedback> FindByMenuItemId(int menuItemId);
        IEnumerable<Feedback> GetAll();
        void DiscardLowRatedItems();
    }
}
