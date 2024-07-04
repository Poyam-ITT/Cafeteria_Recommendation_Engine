namespace RecommendationEngine.Models
{
    public class RolledOutItem
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public string MenuType { get; set; }
        public double RecommendationScore { get; set; }
        public double AverageRating { get; set; }
        public double SentimentScore { get; set; }
        public DateTime RolledOutDate { get; set; }
        public MenuItem MenuItem { get; set; }
    }
}
