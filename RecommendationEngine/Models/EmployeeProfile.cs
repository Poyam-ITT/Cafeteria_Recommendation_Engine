namespace RecommendationEngine.Models
{
    public class EmployeeProfile
    {
        public int UserId { get; set; }
        public string PreferenceType { get; set; }
        public string SpiceLevel { get; set; }
        public string PreferredCuisine { get; set; }
        public bool SweetTooth { get; set; }
    }
}
