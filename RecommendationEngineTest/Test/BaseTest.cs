using Moq;
using NUnit.Framework;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Services;

namespace RecommendationEngineTest.Test
{
    public class BaseTest
    {
        protected Mock<IMenuItemRepository> _menuItemRepositoryMock;
        protected MenuService _menuService;

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
    }
}
