using System;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using RecommendationEngine.Repositories;

namespace RecommendationEngine.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IRolledOutItemRepository _rolledOutItemRepository;

        public EmployeeService(IFeedbackRepository feedbackRepository, IMenuItemRepository menuItemRepository, IRolledOutItemRepository rolledOutItemRepository)
        {
            _feedbackRepository = feedbackRepository;
            _menuItemRepository = menuItemRepository;
            _rolledOutItemRepository = rolledOutItemRepository;
        }

        public void ChooseMenuItem(int menuItemId)
        {
            var menuItem = _menuItemRepository.FindById(menuItemId);
            if (menuItem != null && menuItem.AvailabilityStatus)
            {
                Console.WriteLine($"You have chosen: {menuItem.Name}");
            }
            else
            {
                Console.WriteLine("Menu item is not available.");
            }
        }

        public void GiveFeedback(int menuItemId, int rating, string comment)
        {
            var feedback = new Feedback
            {
                UserId = 1, // Example userId, replace with actual logic
                MenuItemId = menuItemId,
                Rating = rating,
                Comment = comment,
                FeedbackDate = DateTime.Now
            };
            _feedbackRepository.Save(feedback);
        }

        public List<RolledOutItem> ViewRolledOutItems(MenuType menuType)
        {
            return _rolledOutItemRepository.GetRolledOutItems(menuType);
        }
    }
}
