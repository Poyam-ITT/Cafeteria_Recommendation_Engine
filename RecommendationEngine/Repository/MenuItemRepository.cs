using System.Collections.Generic;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using MySql.Data.MySqlClient;

namespace RecommendationEngine.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        public void Save(MenuItem menuItem)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "INSERT INTO MenuItems (Name, Price, AvailabilityStatus, MenuType) VALUES (@Name, @Price, @AvailabilityStatus, @MenuType)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", menuItem.Name);
                    cmd.Parameters.AddWithValue("@Price", menuItem.Price);
                    cmd.Parameters.AddWithValue("@AvailabilityStatus", menuItem.AvailabilityStatus ? 1 : 0);
                    cmd.Parameters.AddWithValue("@MenuType", menuItem.MenuType.ToString());
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(MenuItem menuItem)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "UPDATE MenuItems SET Name = @Name, Price = @Price, AvailabilityStatus = @AvailabilityStatus, MenuType = @MenuType WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", menuItem.Id);
                    cmd.Parameters.AddWithValue("@Name", menuItem.Name);
                    cmd.Parameters.AddWithValue("@Price", menuItem.Price);
                    cmd.Parameters.AddWithValue("@AvailabilityStatus", menuItem.AvailabilityStatus ? 1 : 0);
                    cmd.Parameters.AddWithValue("@MenuType", menuItem.MenuType.ToString());
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public MenuItem FindById(int id)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM MenuItems WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new MenuItem
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Price = reader.GetDecimal("Price"),
                                AvailabilityStatus = reader.GetBoolean("AvailabilityStatus")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Delete(int id)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "DELETE FROM MenuItems WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<MenuItem> GetAll()
        {
            var menuItems = new List<MenuItem>();
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM MenuItems";
                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var menuItem = new MenuItem
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Price = reader.GetDecimal("Price"),
                            AvailabilityStatus = reader.GetBoolean("AvailabilityStatus"),
                            MenuType = Enum.Parse<MenuType>(reader.GetString("MenuType"))
                        };
                        menuItems.Add(menuItem);
                    }
                }
            }
            return menuItems;
        }


        public List<MenuItem> GetItemsByDate(DateTime date)
        {
            var menuItems = new List<MenuItem>();
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM MenuItems WHERE DATE(AvailabilityStatus) = @Date";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Date", date);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            menuItems.Add(new MenuItem
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Price = reader.GetDecimal("Price"),
                                AvailabilityStatus = reader.GetBoolean("AvailabilityStatus")
                            });
                        }
                    }
                }
            }
            return menuItems;
        }
    }
}
