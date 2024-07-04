using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface IChefService
    {
        void RollOutItems(MenuType menuType, List<MenuItem> itemsToRollOut, int userId);
        List<MenuItem> GetRolledOutItems();
        string GenerateMonthlyFeedbackReport();
    }
}
