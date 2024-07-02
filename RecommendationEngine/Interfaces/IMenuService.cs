using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface IMenuService
    {
        void AddMenuItem(MenuItem menuItem);
        void UpdateMenuItem(int id, MenuItem menuItem);
        void DeleteMenuItem(int id);
        List<MenuItem> ViewMenuItems();
        List<MenuItem> GetDiscardMenuItems();
        void RemoveFromDiscardedMenuItems(int id);
    }
}
