﻿namespace RecommendationEngine.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MenuItemId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime FeedbackDate { get; set; }
    }
}
