namespace RecommendationEngine.Models
{
    public class RecommendedItem
    {
        public MenuItem MenuItem { get; set; }
        public double Score { get; set; }
        public double AverageRating { get; set; }
        public double PositiveSentiments { get; set; }
    }
}
