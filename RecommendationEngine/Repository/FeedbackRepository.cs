using System.Collections.Generic;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using MySql.Data.MySqlClient;
using Dapper;

namespace RecommendationEngine.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        public void Save(Feedback feedback)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "INSERT INTO Feedback (UserId, MenuItemId, Rating, Comment, FeedbackDate) VALUES (@UserId, @MenuItemId, @Rating, @Comment, @FeedbackDate)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", feedback.UserId);
                    cmd.Parameters.AddWithValue("@MenuItemId", feedback.MenuItemId);
                    cmd.Parameters.AddWithValue("@Rating", feedback.Rating);
                    cmd.Parameters.AddWithValue("@Comment", feedback.Comment);
                    cmd.Parameters.AddWithValue("@FeedbackDate", feedback.FeedbackDate);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Feedback FindById(int id)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Feedback WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Feedback
                            {
                                Id = reader.GetInt32("Id"),
                                UserId = reader.GetInt32("UserId"),
                                MenuItemId = reader.GetInt32("MenuItemId"),
                                Rating = reader.GetInt32("Rating"),
                                Comment = reader.GetString("Comment"),
                                FeedbackDate = reader.GetDateTime("FeedbackDate")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<Feedback> FindByMenuItemId(int menuItemId)
        {
            var feedbacks = new List<Feedback>();
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Feedback WHERE MenuItemId = @MenuItemId";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            feedbacks.Add(new Feedback
                            {
                                Id = reader.GetInt32("Id"),
                                UserId = reader.GetInt32("UserId"),
                                MenuItemId = reader.GetInt32("MenuItemId"),
                                Rating = reader.GetInt32("Rating"),
                                Comment = reader.GetString("Comment"),
                                FeedbackDate = reader.GetDateTime("FeedbackDate")
                            });
                        }
                    }
                }
            }
            return feedbacks;
        }

        public IEnumerable<Feedback> GetAll()
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                return connection.Query<Feedback>("SELECT * FROM Feedback");
            }
        }
    }
}
