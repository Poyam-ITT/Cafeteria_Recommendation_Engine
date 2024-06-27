using System;
using System.Collections.Generic;
using System.Linq;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using RecommendationEngine.Repositories;

namespace RecommendationEngine.Services
{
    public class ChefService : IChefService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        //private readonly INotificationService _notificationService;
        private List<MenuItem> rolledOutItems;
        private readonly IRecommendationEngine _recommendationEngine;
        private readonly IRolledOutItemRepository _rolledOutItemRepository;

        public ChefService(IRecommendationEngine recommendationEngine, IRolledOutItemRepository rolledOutItemRepository, IMenuItemRepository menuItemRepository)
        {
            _recommendationEngine = recommendationEngine;
            _rolledOutItemRepository = rolledOutItemRepository;
            _menuItemRepository = menuItemRepository;
            //_notificationService = notificationService;
            rolledOutItems = new List<MenuItem>();
        }

        public void RollOutItems(MenuType menuType, int itemCount)
        {
            var recommendedItems = _recommendationEngine.GetFoodItemForNextDay(menuType, itemCount);
            var rolledOutItems = new List<RolledOutItem>();

            foreach (var recommendedItem in recommendedItems)
            {
                rolledOutItems.Add(new RolledOutItem
                {
                    MenuItemId = recommendedItem.MenuItem.Id,
                    MenuType = menuType.ToString(), 
                    RecommendationScore = recommendedItem.Score,
                    AverageRating = recommendedItem.AverageRating,
                    SentimentScore = recommendedItem.PositiveSentiments,
                    RolledOutDate = DateTime.Now,
                    MenuItem = recommendedItem.MenuItem
                });
            }

            _rolledOutItemRepository.AddRolledOutItems(rolledOutItems);
            //_notificationService.SendNotification("New menu items rolled out for tomorrow!", recommendedItems.Select(r => r.MenuItem).ToList());
        }

        public List<MenuItem> GetRolledOutItems()
        {
            return rolledOutItems;
        }

        public string GenerateMonthlyFeedbackReport()
        {
            var feedbacks = new List<Feedback>();
            foreach (var item in rolledOutItems)
            {
                var itemFeedbacks = item.Feedbacks;
                if (itemFeedbacks != null)
                {
                    feedbacks.AddRange(itemFeedbacks);
                }
            }

            var groupedFeedbacks = feedbacks.GroupBy(f => f.MenuItemId);
            var report = "Monthly Feedback Report:\n";
            foreach (var group in groupedFeedbacks)
            {
                var menuItem = _menuItemRepository.FindById(group.Key);
                var averageRating = group.Average(f => f.Rating);
                report += $"Item: {menuItem.Name}, Average Rating: {averageRating}\n";
            }

            return report;
        }
    }
}
