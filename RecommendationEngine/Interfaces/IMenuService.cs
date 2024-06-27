using System.Collections.Generic;
using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface IMenuService
    {
        void AddMenuItem(string name, decimal price, bool availabilityStatus);
        void UpdateMenuItem(int id, string name, decimal price, bool availabilityStatus);
        void DeleteMenuItem(int id);
        List<MenuItem> ViewMenuItems(); 
    }
}
