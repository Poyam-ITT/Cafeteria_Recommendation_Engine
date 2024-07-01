using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using System.Collections.Generic;

namespace RecommendationEngine.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuService(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        public void AddMenuItem(MenuItem menuItem)
        {
            _menuItemRepository.Save(menuItem);
        }

        public void UpdateMenuItem(int id, MenuItem menuItem)
        {
            var existingMenuItem = _menuItemRepository.FindById(id);
            if (existingMenuItem != null)
            {
                existingMenuItem.Name = menuItem.Name;
                existingMenuItem.Price = menuItem.Price;
                existingMenuItem.AvailabilityStatus = menuItem.AvailabilityStatus;
                existingMenuItem.MenuType = menuItem.MenuType;
                existingMenuItem.IsVegetarian = menuItem.IsVegetarian;
                existingMenuItem.IsNonVegetarian = menuItem.IsNonVegetarian;
                existingMenuItem.IsEggetarian = menuItem.IsEggetarian;
                existingMenuItem.SpiceLevel = menuItem.SpiceLevel;
                _menuItemRepository.Update(existingMenuItem);
            }
        }

        public void DeleteMenuItem(int id)
        {
            _menuItemRepository.Delete(id);
        }

        public List<MenuItem> ViewMenuItems()
        {
            return _menuItemRepository.GetAll();
        }
    }
}
