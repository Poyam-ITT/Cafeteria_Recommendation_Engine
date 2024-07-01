using MySql.Data.MySqlClient;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;

namespace RecommendationEngine.Repository
{
    public class EmployeeProfileRepository : IEmployeeProfileRepository
    {
        public void Save(EmployeeProfile profile)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "INSERT INTO EmployeeProfile (UserId, PreferenceType, SpiceLevel, PreferredCuisine, SweetTooth) VALUES (@UserId, @PreferenceType, @SpiceLevel, @PreferredCuisine, @SweetTooth)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", profile.UserId);
                    cmd.Parameters.AddWithValue("@PreferenceType", profile.PreferenceType);
                    cmd.Parameters.AddWithValue("@SpiceLevel", profile.SpiceLevel);
                    cmd.Parameters.AddWithValue("@PreferredCuisine", profile.PreferredCuisine);
                    cmd.Parameters.AddWithValue("@SweetTooth", profile.SweetTooth);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public EmployeeProfile FindByUserId(int userId)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM EmployeeProfile WHERE UserId = @UserId";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new EmployeeProfile
                            {
                                UserId = reader.GetInt32("UserId"),
                                PreferenceType = reader.GetString("PreferenceType"),
                                SpiceLevel = reader.GetString("SpiceLevel"),
                                PreferredCuisine = reader.GetString("PreferredCuisine"),
                                SweetTooth = reader.GetBoolean("SweetTooth")
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
