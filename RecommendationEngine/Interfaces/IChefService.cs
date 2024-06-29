using System.Collections.Generic;
using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface IChefService
    {
        void RollOutItems(MenuType menuType, List<MenuItem> itemsToRollOut);
        public List<RecommendedItem> GetFoodItemForNextDay(MenuType menuType, int returnItemListSize);
        List<MenuItem> GetRolledOutItems();
        string GenerateMonthlyFeedbackReport();
    }
}
