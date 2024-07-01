namespace RecommendationEngine.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool AvailabilityStatus { get; set; }
        public List<Feedback> Feedbacks { get; set; }
        public int PreparationCount { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsNonVegetarian { get; set; }
        public bool IsEggetarian { get; set; }
        public string SpiceLevel { get; set; }
        public MenuType MenuType { get; set; }

    }
}
