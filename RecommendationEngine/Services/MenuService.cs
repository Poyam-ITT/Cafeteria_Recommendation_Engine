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

        public void AddMenuItem(string name, decimal price, bool availabilityStatus, MenuType menuType)
        {
            var menuItem = new MenuItem
            {
                Name = name,
                Price = price,
                AvailabilityStatus = availabilityStatus,
                MenuType = menuType
            };
            _menuItemRepository.Save(menuItem);
        }

        public void UpdateMenuItem(int id, string name, decimal price, bool availabilityStatus, MenuType menuType)
        {
            var menuItem = _menuItemRepository.FindById(id);
            if (menuItem != null)
            {
                menuItem.Name = name;
                menuItem.Price = price;
                menuItem.AvailabilityStatus = availabilityStatus;
                menuItem.MenuType = menuType;
                _menuItemRepository.Update(menuItem);
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
