using System.Collections.Generic;
using RecommendationEngine.Models;
using static Google.Protobuf.Reflection.FeatureSet.Types;

namespace RecommendationEngine.Interfaces
{
    public interface IRolledOutItemRepository
    {
        void AddRolledOutItems(List<RolledOutItem> items);
        List<RolledOutItem> GetRolledOutItems(MenuType menuType);
    }
}
