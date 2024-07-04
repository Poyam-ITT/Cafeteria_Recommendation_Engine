using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface IRecommendationEngine
    {
        List<RecommendedItem> GetFoodItemForNextDay(MenuType menuType, int returnItemListSize, int userId);
    }
}
