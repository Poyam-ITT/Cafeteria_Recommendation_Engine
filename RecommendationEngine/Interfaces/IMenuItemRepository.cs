using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface IMenuItemRepository
    {
        void Save(MenuItem menuItem);
        MenuItem FindById(int id);
        void Update(MenuItem menuItem);
        void Delete(int id);
        List<MenuItem> GetAll();
        List<MenuItem> GetDiscardMenuItems();
        void RemoveItemFromDiscardedMenuItems(int id);
    }
}
