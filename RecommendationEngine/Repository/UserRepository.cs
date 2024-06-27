using System;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using MySql.Data.MySqlClient;

namespace RecommendationEngine.Repositories
{
    public class UserRepository : IUserRepository
    {
        public void Save(User user)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "INSERT INTO Users (EmployeeId, Name, Role) VALUES (@EmployeeId, @Name, @Role)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", user.EmployeeId);
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public User FindById(int id)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Users WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("Id"),
                                EmployeeId = reader.GetString("EmployeeId"),
                                Name = reader.GetString("Name"),
                                Role = reader.GetString("Role")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public User FindByEmployeeIdAndName(string employeeId, string name)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Users WHERE EmployeeId = @EmployeeId AND Name = @Name";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@Name", name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("Id"),
                                EmployeeId = reader.GetString("EmployeeId"),
                                Name = reader.GetString("Name"),
                                Role = reader.GetString("Role")
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
