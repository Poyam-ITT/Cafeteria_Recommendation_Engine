using Moq;
using NUnit.Framework;
using RecommendationEngine.Models;
using RecommendationEngineTest.Test;

namespace RecommendationEngine.Tests.Services
{
    public class MenuServiceTests : BaseTest
    {
        [Test]
        public void VerifyUpdateMenuItemUpdatesExistingMenuItem()
        {
            var existingMenuItem = new MenuItem { Id = MenuItemConstant.MenuItemId, Name = "Uttpam", Price = 40 };

            _menuItemRepositoryMock.Setup(menuItemRepo => menuItemRepo.FindById(MenuItemConstant.MenuItemId)).Returns(existingMenuItem);

            _menuService.UpdateMenuItem(MenuItemConstant.MenuItemId, MenuItemConstant.TestMenuItem);

            _menuItemRepositoryMock.Verify(menuItemRepo => menuItemRepo.Update(It.Is<MenuItem>(item =>
                item.Id == MenuItemConstant.MenuItemId &&
                item.Name == MenuItemConstant.TestMenuItem.Name &&
                item.Price == MenuItemConstant.TestMenuItem.Price &&
                item.AvailabilityStatus == MenuItemConstant.TestMenuItem.AvailabilityStatus &&
                item.MenuType == MenuItemConstant.TestMenuItem.MenuType &&
                item.IsVegetarian == MenuItemConstant.TestMenuItem.IsVegetarian &&
                item.IsNonVegetarian == MenuItemConstant.TestMenuItem.IsNonVegetarian &&
                item.IsEggetarian == MenuItemConstant.TestMenuItem.IsEggetarian &&
                item.SpiceLevel == MenuItemConstant.TestMenuItem.SpiceLevel
            )), Times.Once);

            AssertWrapper.AssertExpectedMatchesActual(MenuItemConstant.TestMenuItem.Name, existingMenuItem.Name, "Menu Item name doesn't match");
            AssertWrapper.AssertExpectedMatchesActual(MenuItemConstant.TestMenuItem.Price, existingMenuItem.Price, "Menu Item price doesn't match");
        }

        [Test]
        public void VerifyViewMenuItemsReturnMenuItemList()
        {
            var menuItems = new List<MenuItem>
            {
                new MenuItem { Id = MenuItemConstant.MenuItemId, Name = MenuItemConstant.MenuItemName, Price = MenuItemConstant.MenuItemPrice },
                new MenuItem { Id = 2, Name = "Upma", Price = 40 }
            };

            _menuItemRepositoryMock.Setup(menuItemRepo => menuItemRepo.GetAll()).Returns(menuItems);

            var result = _menuService.ViewMenuItems();

            AssertWrapper.AssertExpectedMatchesActual(2, result.Count, "Item Count is not updated");
            AssertWrapper.AssertExpectedMatchesActual(MenuItemConstant.MenuItemName, result[0].Name, "Name for item 1 doesn't match");
            AssertWrapper.AssertExpectedMatchesActual("Upma", result[1].Name, "Name for item 2 doesn't match");
        }

        [Test]
        public void VerifyAddMenuItem()
        {
            var menuItemToAdd = new MenuItem { Id = 3, Name = "Sandwich", Price = 50 };

            _menuItemRepositoryMock.Setup(menuItemRepo => menuItemRepo.Save(It.IsAny<MenuItem>()));

            _menuService.AddMenuItem(menuItemToAdd);

            _menuItemRepositoryMock.Verify(repo => repo.Save(It.Is<MenuItem>(m =>
                m.Id == menuItemToAdd.Id &&
                m.Name == menuItemToAdd.Name &&
                m.Price == menuItemToAdd.Price)), Times.Once);
        }

        [Test]
        public void VerifyViewDiscardedMenuItemsReturnDiscardedMenuItemList()
        {
            var discardedMenuItems = new List<MenuItem>
            {
                new MenuItem { Id = MenuItemConstant.MenuItemId, Name = "Sambhar Vada", Price = 70 }
            };

            _menuItemRepositoryMock.Setup(menuItemRepo => menuItemRepo.GetDiscardMenuItems()).Returns(discardedMenuItems);

            var result = _menuService.GetDiscardMenuItems();

            AssertWrapper.AssertExpectedMatchesActual(1, result.Count, "Item Count is not updated");
            AssertWrapper.AssertExpectedMatchesActual("Sambhar Vada", result[0].Name, "Name for item doesn't match");
        }

        [Test]
        public void VerifyUpdateMenuItemWithInvalidId()
        {
            _menuItemRepositoryMock.Setup(menuItemRepo => menuItemRepo.FindById(222222)).Returns((MenuItem)null);

            _menuService.UpdateMenuItem(MenuItemConstant.MenuItemId, MenuItemConstant.TestMenuItem);

            _menuItemRepositoryMock.Verify(menuItemRepo => menuItemRepo.Update(It.IsAny<MenuItem>()), Times.Never);

            AssertWrapper.AssertExpectedMatchesActual(MenuItemConstant.MenuItemName, MenuItemConstant.TestMenuItem.Name, "Menu Item name changed");
            AssertWrapper.AssertExpectedMatchesActual(MenuItemConstant.MenuItemPrice, MenuItemConstant.TestMenuItem.Price, "Menu Item price changed");
        }
    }
}
