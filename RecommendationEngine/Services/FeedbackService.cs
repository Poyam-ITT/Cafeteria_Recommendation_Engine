using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;

namespace RecommendationEngine.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public void GiveFeedback(int userId, int menuItemId, int rating, string comment)
        {
            var feedback = new Feedback
            {
                UserId = userId,
                MenuItemId = menuItemId,
                Rating = rating,
                Comment = comment,
                FeedbackDate = DateTime.Now
            };
            _feedbackRepository.Save(feedback);
        }

        public IEnumerable<Feedback> GetFeedbacksByMenuItemId(int menuItemId)
        {
            return _feedbackRepository.FindByMenuItemId(menuItemId);
        }

        public void MoveLowRatedItemsToDiscardedList()
        {
            _feedbackRepository.DiscardLowRatedItems();
        }

        public IEnumerable<Feedback> GetAllFeedbacks()
        {
            return _feedbackRepository.GetAll();
        }
    }
}
