using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;

namespace RecommendationEngine.Services
{
    public class RecommendationEngine : IRecommendationEngine
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ISentimentAnalysisService _sentimentAnalysisService;
        private readonly IEmployeeProfileService _profileService;

        public RecommendationEngine(IMenuItemRepository menuItemRepository, IFeedbackRepository feedbackRepository, ISentimentAnalysisService sentimentAnalysisService, IEmployeeProfileService profileService)
        {
            _menuItemRepository = menuItemRepository;
            _feedbackRepository = feedbackRepository;
            _sentimentAnalysisService = sentimentAnalysisService;
            _profileService = profileService;
        }

        public List<RecommendedItem> GetFoodItemForNextDay(MenuType menuType, int returnItemListSize, int userId)
        {
            var profile = _profileService.GetProfile(userId);
            var allMenuItems = _menuItemRepository.GetAll();
            var filteredItems = allMenuItems.Where(item => item.MenuType == menuType).ToList();
            var feedbacks = _feedbackRepository.GetAll();

            // Sort and filter based on profile preferences
            if (profile != null)
            {
                filteredItems = filteredItems
                    .Where(item =>
                        (profile.PreferenceType == "Vegetarian" && item.IsVegetarian) ||
                        (profile.PreferenceType == "Non Vegetarian" && item.IsNonVegetarian) ||
                        (profile.PreferenceType == "Eggetarian" && item.IsEggetarian))
                    .OrderByDescending(item =>
                        (profile.SpiceLevel == "High" && item.SpiceLevel == "High") ? 1 :
                        (profile.SpiceLevel == "Medium" && item.SpiceLevel == "Medium") ? 1 :
                        (profile.SpiceLevel == "Low" && item.SpiceLevel == "Low") ? 1 : 0)
                    .ToList();
            }

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
