using System.Collections.Generic;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using MySql.Data.MySqlClient;

namespace RecommendationEngine.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        public void Save(Notification notification)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "INSERT INTO Notifications (UserId, Message, Type, Date) VALUES (@UserId, @Message, @Type, @Date)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", notification.UserId);
                    cmd.Parameters.AddWithValue("@Message", notification.Message);
                    cmd.Parameters.AddWithValue("@Type", notification.Type);
                    cmd.Parameters.AddWithValue("@Date", notification.Date);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Notification FindById(int id)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Notifications WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Notification
                            {
                                Id = reader.GetInt32("Id"),
                                UserId = reader.GetInt32("UserId"),
                                Message = reader.GetString("Message"),
                                Type = reader.GetString("Type"),
                                Date = reader.GetDateTime("Date")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<Notification> FindByUserId(int userId)
        {
            var notifications = new List<Notification>();
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Notifications WHERE UserId = @UserId";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notifications.Add(new Notification
                            {
                                Id = reader.GetInt32("Id"),
                                UserId = reader.GetInt32("UserId"),
                                Message = reader.GetString("Message"),
                                Type = reader.GetString("Type"),
                                Date = reader.GetDateTime("Date")
                            });
                        }
                    }
                }
            }
            return notifications;
        }
    }
}
