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
            var existingMenuItem = new MenuItem { Id = TestConstants.MenuItemId, Name = "Uttpam", Price = 40 };

            _menuItemRepositoryMock.Setup(menuItemRepo => menuItemRepo.FindById(TestConstants.MenuItemId)).Returns(existingMenuItem);

            _menuService.UpdateMenuItem(TestConstants.MenuItemId, TestConstants.TestMenuItem);

            _menuItemRepositoryMock.Verify(menuItemRepo => menuItemRepo.Update(It.Is<MenuItem>(item =>
                item.Id == TestConstants.MenuItemId &&
                item.Name == TestConstants.TestMenuItem.Name &&
                item.Price == TestConstants.TestMenuItem.Price &&
                item.AvailabilityStatus == TestConstants.TestMenuItem.AvailabilityStatus &&
                item.MenuType == TestConstants.TestMenuItem.MenuType &&
                item.IsVegetarian == TestConstants.TestMenuItem.IsVegetarian &&
                item.IsNonVegetarian == TestConstants.TestMenuItem.IsNonVegetarian &&
                item.IsEggetarian == TestConstants.TestMenuItem.IsEggetarian &&
                item.SpiceLevel == TestConstants.TestMenuItem.SpiceLevel
            )), Times.Once);

            AssertWrapper.AssertExpectedMatchesActual(TestConstants.TestMenuItem.Name, existingMenuItem.Name, "Menu Item name doesn't match");
            AssertWrapper.AssertExpectedMatchesActual(TestConstants.TestMenuItem.Price, existingMenuItem.Price, "Menu Item price doesn't match");
        }

        [Test]
        public void VerifyViewMenuItemsReturnMenuItemList()
        {
            var menuItems = new List<MenuItem>
            {
                new MenuItem { Id = TestConstants.MenuItemId, Name = TestConstants.MenuItemName, Price = TestConstants.MenuItemPrice },
                new MenuItem { Id = 2, Name = "Upma", Price = 40 }
            };

            _menuItemRepositoryMock.Setup(menuItemRepo => menuItemRepo.GetAll()).Returns(menuItems);

            var result = _menuService.ViewMenuItems();

            AssertWrapper.AssertExpectedMatchesActual(2, result.Count, "Item Count is not updated");
            AssertWrapper.AssertExpectedMatchesActual(TestConstants.MenuItemName, result[0].Name, "Name for item 1 doesn't match");
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
                new MenuItem { Id = TestConstants.MenuItemId, Name = "Sambhar Vada", Price = 70 }
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

            _menuService.UpdateMenuItem(TestConstants.MenuItemId, TestConstants.TestMenuItem);

            _menuItemRepositoryMock.Verify(menuItemRepo => menuItemRepo.Update(It.IsAny<MenuItem>()), Times.Never);

            AssertWrapper.AssertExpectedMatchesActual(TestConstants.MenuItemName, TestConstants.TestMenuItem.Name, "Menu Item name changed");
            AssertWrapper.AssertExpectedMatchesActual(TestConstants.MenuItemPrice, TestConstants.TestMenuItem.Price, "Menu Item price changed");
        }
    }
}
