using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface IEmployeeService
    {
        void ChooseMenuItem(int menuItemId);
        void GiveFeedback(int menuItemId, int rating, string comment);
        List<RolledOutItem> ViewRolledOutItems(MenuType menuType);
    }
}
