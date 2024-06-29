using System.Collections.Generic;
using System.Linq;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;

namespace RecommendationEngine.Services
{
    public class RecommendationEngine : IRecommendationEngine
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ISentimentAnalysisService _sentimentAnalysisService;

        public RecommendationEngine(IMenuItemRepository menuItemRepository, IFeedbackRepository feedbackRepository, ISentimentAnalysisService sentimentAnalysisService)
        {
            _menuItemRepository = menuItemRepository;
            _feedbackRepository = feedbackRepository;
            _sentimentAnalysisService = sentimentAnalysisService;
        }

        public List<RecommendedItem> GetFoodItemForNextDay(MenuType menuType, int returnItemListSize)
        {
            var allMenuItems = _menuItemRepository.GetAll();
            var filteredItems = allMenuItems.Where(item => item.MenuType == menuType).ToList();
            var feedbacks = _feedbackRepository.GetAll();

            var scoredItems = filteredItems.Select(item =>
            {
                var itemFeedbacks = feedbacks.Where(f => f.MenuItemId == item.Id).ToList();
                var averageRating = itemFeedbacks.Any() ? itemFeedbacks.Average(f => f.Rating) : 0;
                var sentimentScores = itemFeedbacks.Select(f => _sentimentAnalysisService.AnalyzeSentiment(f.Comment)).ToList();
                var positiveSentiments = sentimentScores.Count(s => s > 0);

                return new RecommendedItem
                {
                    MenuItem = item,
                    Score = (item.PreparationCount * 0.4) + (averageRating * 0.3) + (positiveSentiments * 0.3),
                    AverageRating = averageRating,
                    PositiveSentiments = positiveSentiments
                };
            }).OrderByDescending(i => i.Score).ToList();

            return scoredItems.Take(returnItemListSize).ToList();
        }
    }
}
