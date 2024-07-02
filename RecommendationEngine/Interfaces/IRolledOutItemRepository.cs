using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface IRolledOutItemRepository
    {
        void AddRolledOutItems(List<RolledOutItem> items);
        List<RolledOutItem> GetRolledOutItems(MenuType menuType);
    }
}
