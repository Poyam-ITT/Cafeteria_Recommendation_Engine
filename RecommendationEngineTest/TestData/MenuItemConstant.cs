using RecommendationEngine.Models;

public static class MenuItemConstant
{
    public const int MenuItemId = 1;
    public const string MenuItemName = "Dosa";
    public const decimal MenuItemPrice = 55;

    public static readonly MenuItem TestMenuItem = new MenuItem
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
}
