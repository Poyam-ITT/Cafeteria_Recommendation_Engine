namespace RecommendationEngine.Interfaces
{
    public interface IFeedbackService
    {
        void GiveFeedback(int userId, int menuItemId, int rating, string comment);
        void MoveLowRatedItemsToDiscardedList();
    }
}
