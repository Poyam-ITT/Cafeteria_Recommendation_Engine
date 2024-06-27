using MySql.Data.MySqlClient;
using System;

namespace RecommendationEngine.Utils
{
    public static class DbUtils
    {
        public static void InitializeDatabase()
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var createUsersTable = @"CREATE TABLE IF NOT EXISTS Users (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    EmployeeId VARCHAR(50) NOT NULL,
                    Name VARCHAR(100) NOT NULL,
                    Role VARCHAR(50) NOT NULL
                )";

                var createMenuItemsTable = @"CREATE TABLE IF NOT EXISTS MenuItems (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Name VARCHAR(100) NOT NULL,
                    Price DECIMAL(5, 2) NOT NULL,
                    AvailabilityStatus BOOLEAN NOT NULL
                )";

                var createFeedbackTable = @"CREATE TABLE IF NOT EXISTS Feedback (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    UserId INT,
                    MenuItemId INT,
                    Rating INT,
                    Comment TEXT,
                    FeedbackDate DATE,
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    FOREIGN KEY (MenuItemId) REFERENCES MenuItems(Id)
                )";

                using (var cmd = new MySqlCommand(createUsersTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new MySqlCommand(createMenuItemsTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new MySqlCommand(createFeedbackTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
