using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;

namespace RecommendationEngine.Services
{
    public class ChefService : IChefService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private List<MenuItem> rolledOutItems;
        private readonly IRecommendationEngine _recommendationEngine;
        private readonly IRolledOutItemRepository _rolledOutItemRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ISentimentAnalysisService _sentimentAnalysisService;

        public ChefService(IRecommendationEngine recommendationEngine, IRolledOutItemRepository rolledOutItemRepository, IMenuItemRepository menuItemRepository, IFeedbackRepository feedbackRepository, ISentimentAnalysisService sentimentAnalysisService)
        {
            _recommendationEngine = recommendationEngine;
            _rolledOutItemRepository = rolledOutItemRepository;
            _menuItemRepository = menuItemRepository;
            rolledOutItems = new List<MenuItem>();
            _feedbackRepository = feedbackRepository;
            _sentimentAnalysisService = sentimentAnalysisService;
        }

        public void RollOutItems(MenuType menuType, List<MenuItem> itemsToRollOut, int userId)
        {
            var rolledOutItems = new List<RolledOutItem>();

            foreach (var item in itemsToRollOut)
            {
                var recommendedItem = _recommendationEngine.GetFoodItemForNextDay(menuType, itemsToRollOut.Count, userId)
                                    .FirstOrDefault(r => r.MenuItem.Id == item.Id);

                if (recommendedItem != null)
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
            }

            _rolledOutItemRepository.AddRolledOutItems(rolledOutItems);
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
