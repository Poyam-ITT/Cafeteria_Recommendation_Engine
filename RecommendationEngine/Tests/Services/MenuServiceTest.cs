using Moq;
using NUnit.Framework;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using RecommendationEngine.Services;

namespace RecommendationEngine.Tests.Services
{
    [TestFixture]
    public class MenuServiceTests
    {
        private Mock<IMenuItemRepository> _menuItemRepositoryMock;
        private MenuService _menuService;

        private const int MenuItemId = 1;
        private const string MenuItemName = "Dosa";
        private const decimal MenuItemPrice = 55;
        private static readonly MenuItem TestMenuItem = new MenuItem
        {
            Name = "Dosa",
            Price = 55,
            AvailabilityStatus = true,
            MenuType = MenuType.Breakfast,
            IsVegetarian = true,
            IsNonVegetarian = false,
            IsEggetarian = false,
            SpiceLevel = "Low"
        };

        [SetUp]
        public void Setup()
        {
            _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
            _menuService = new MenuService(_menuItemRepositoryMock.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _menuItemRepositoryMock = null;
            _menuService = null;
        }

        [Test]
        public void VerifyUpdateMenuItemUpdatesExistingMenuItem()
        {
            var existingMenuItem = new MenuItem { Id = MenuItemId, Name = "Uttpam", Price = 40 };

            _menuItemRepositoryMock.Setup(menuItemRepo => menuItemRepo.FindById(MenuItemId)).Returns(existingMenuItem);

            _menuService.UpdateMenuItem(MenuItemId, TestMenuItem);

            _menuItemRepositoryMock.Verify(menuItemRepo => menuItemRepo.Update(It.Is<MenuItem>(item =>
                item.Id == MenuItemId &&
                item.Name == TestMenuItem.Name &&
                item.Price == TestMenuItem.Price &&
                item.AvailabilityStatus == TestMenuItem.AvailabilityStatus &&
                item.MenuType == TestMenuItem.MenuType &&
                item.IsVegetarian == TestMenuItem.IsVegetarian &&
                item.IsNonVegetarian == TestMenuItem.IsNonVegetarian &&
                item.IsEggetarian == TestMenuItem.IsEggetarian &&
                item.SpiceLevel == TestMenuItem.SpiceLevel
            )), Times.Once);

            Assert.That(existingMenuItem.Name, Is.EqualTo(TestMenuItem.Name));
            Assert.That(existingMenuItem.Price, Is.EqualTo(TestMenuItem.Price));
        }

        [Test]
        public void VerifyUpdateMenuItemWithInvalidId()
        {
            _menuItemRepositoryMock.Setup(menuItemRepo => menuItemRepo.FindById(MenuItemId)).Returns((MenuItem)null);

            _menuService.UpdateMenuItem(MenuItemId, TestMenuItem);

            _menuItemRepositoryMock.Verify(menuItemRepo => menuItemRepo.Update(It.IsAny<MenuItem>()), Times.Never);

            Assert.That(TestMenuItem.Name, Is.EqualTo("Dosa"), "Menu Item name changed");
            Assert.That(TestMenuItem.Price, Is.EqualTo(55), "Menu Item price changed");
        }

        [Test]
        public void VerifyViewMenuItemsReturnMenuItemList()
        {
            var menuItems = new List<MenuItem>
            {
                new MenuItem { Id = MenuItemId, Name = MenuItemName, Price = MenuItemPrice },
                new MenuItem { Id = 2, Name = "Upma", Price = 40 }
            };

            _menuItemRepositoryMock.Setup(menuItemRepo => menuItemRepo.GetAll()).Returns(menuItems);

            var result = _menuService.ViewMenuItems();

            Assert.That(result.Count, Is.EqualTo(2), "Item Count is not updated");
            Assert.That(result[0].Name, Is.EqualTo(MenuItemName), "Name for item 1 doesn't match");
            Assert.That(result[1].Name, Is.EqualTo("Upma"), "Name for item 2 doesn't match");
        }

        [Test]
        public void VerifyViewDiscardedMenuItemsReturnDiscardedMenuItemList()
        {
            var discardedMenuItems = new List<MenuItem>
            {
                new MenuItem { Id = MenuItemId, Name = "Sambhar Vada", Price = 70 }
            };

            _menuItemRepositoryMock.Setup(menuItemRepo => menuItemRepo.GetDiscardMenuItems()).Returns(discardedMenuItems);

            var result = _menuService.GetDiscardMenuItems();

            Assert.That(result.Count, Is.EqualTo(1), "Item Count is not updated");
            Assert.That(result[0].Name, Is.EqualTo("Sambhar Vada"), "Name for item doesn't match");
        }
    }
}
