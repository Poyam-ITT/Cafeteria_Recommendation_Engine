using System.Collections.Generic;
using RecommendationEngine.Models;
using static Google.Protobuf.Reflection.FeatureSet.Types;

namespace RecommendationEngine.Interfaces
{
    public interface IRecommendationEngine
    {
        List<RecommendedItem> GetFoodItemForNextDay(MenuType menuType, int returnItemListSize, int userId);
    }
}
