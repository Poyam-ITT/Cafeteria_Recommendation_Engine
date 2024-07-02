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

        public void DiscardLowRatedItems()
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();

                var query = @"
                SELECT f.MenuItemId, mi.Name, mi.Price, mi.AvailabilityStatus, mi.MenuType, 
                       mi.IsVegetarian, mi.IsNonVegetarian, mi.IsEggetarian, mi.SpiceLevel, 
                       AVG(f.Rating) as AverageRating
                       FROM Feedback f
                       JOIN MenuItems mi ON f.MenuItemId = mi.Id
                       WHERE f.Rating < 2
                       GROUP BY f.MenuItemId, mi.Name, mi.Price, mi.AvailabilityStatus, mi.MenuType, 
                       mi.IsVegetarian, mi.IsNonVegetarian, mi.IsEggetarian, mi.SpiceLevel";

                var lowRatedItems = connection.Query(query);

                foreach (var item in lowRatedItems)
                {
                    var insertQuery = @"
                    INSERT INTO discardedmenuitems (MenuItemId, Name, Price, AvailabilityStatus, MenuType, 
                                                    IsVegetarian, IsNonVegetarian, IsEggetarian, SpiceLevel, 
                                                    AverageRating, DiscardedDate)
                                                    VALUES (@MenuItemId, @Name, @Price, @AvailabilityStatus, @MenuType, 
                                                    @IsVegetarian, @IsNonVegetarian, @IsEggetarian, @SpiceLevel, 
                                                    @AverageRating, @DiscardedDate)";

                    connection.Execute(insertQuery, new
                    {
                        MenuItemId = item.MenuItemId,
                        Name = item.Name,
                        Price = item.Price,
                        AvailabilityStatus = item.AvailabilityStatus,
                        MenuType = item.MenuType,
                        IsVegetarian = item.IsVegetarian,
                        IsNonVegetarian = item.IsNonVegetarian,
                        IsEggetarian = item.IsEggetarian,
                        SpiceLevel = item.SpiceLevel,
                        AverageRating = item.AverageRating,
                        DiscardedDate = DateTime.Now
                    });
                }
            }
        }
    }
}
