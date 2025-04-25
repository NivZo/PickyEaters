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
    // Brown,
}

public static class FoodTypeExtensions
{
    public static NamedColor GetNamedColor(this FoodType foodType)
    {
        return foodType switch
        {
            FoodType.White => NamedColor.White,
            FoodType.Green => NamedColor.Green,
            FoodType.Blue => NamedColor.Blue,
            FoodType.Red => NamedColor.Red,
            FoodType.Yellow => NamedColor.Yellow,
            FoodType.Purple => NamedColor.Purple,
            FoodType.Pink => NamedColor.Pink,
            // FoodType.Brown => NamedColor.Brown,
            _ => NamedColor.Gray
        };
    }
    public static Texture2D GetFoodTypeTexture(this FoodType foodType, bool isLast)
        => GD.Load<Texture2D>($"Assets/Entities/{foodType.ToString().ToLowerInvariant()}_{(isLast ? "star_" : string.Empty)}food.png");
}