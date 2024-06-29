using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;

namespace RecommendationEngine.Repositories
{
    public class RolledOutItemRepository : IRolledOutItemRepository
    {
        private readonly string _connectionString;

        public RolledOutItemRepository()
        {
            _connectionString = AppConfig.ConnectionString;
        }

        public void AddRolledOutItems(List<RolledOutItem> items)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                foreach (var item in items)
                {
                    connection.Execute("INSERT INTO RolledOutItems (MenuItemId, MenuType, RecommendationScore, AverageRating, SentimentScore, RolledOutDate) VALUES (@MenuItemId, @MenuType, @RecommendationScore, @AverageRating, @SentimentScore, @RolledOutDate)",
                        new { item.MenuItemId, item.MenuType, item.RecommendationScore, item.AverageRating, item.SentimentScore, item.RolledOutDate });
                }
            }
        }

        public List<RolledOutItem> GetRolledOutItems(MenuType menuType)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
                            SELECT roi.*, mi.Name, mi.Price, roi.RecommendationScore, roi.AverageRating, roi.SentimentScore
                            FROM RolledOutItems roi 
                            JOIN MenuItems mi ON roi.MenuItemId = mi.Id 
                            WHERE roi.MenuType = @MenuType";
                return connection.Query<RolledOutItem, MenuItem, RolledOutItem>(
                    query,
                    (rolledOutItem, menuItem) =>
                    {
                        rolledOutItem.MenuItem = menuItem;
                        return rolledOutItem;
                    },
                    new { MenuType = menuType.ToString() },
                    splitOn: "MenuItemId"
                ).AsList();
            }
        }
    }
}
