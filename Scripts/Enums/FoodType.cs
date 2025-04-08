using Godot;

public enum FoodType
{
    White,
    Green,
    Blue,
    Red,
    Yellow,
    Purple,
    Pink,
    Brown,
}

public static class FoodTypeExtensions
{
    public static Texture2D GetFoodTypeTexture(this FoodType foodType, bool isLast)
        => GD.Load<Texture2D>($"Assets/Entities/{foodType.ToString().ToLowerInvariant()}_{(isLast ? "star_" : string.Empty)}food.png");
}