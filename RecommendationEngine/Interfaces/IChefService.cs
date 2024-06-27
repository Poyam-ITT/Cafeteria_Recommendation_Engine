using System.Collections.Generic;
using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface IChefService
    {
        void RollOutItems(MenuType menuType, int itemCount);
        List<MenuItem> GetRolledOutItems();
        string GenerateMonthlyFeedbackReport();
    }
}
