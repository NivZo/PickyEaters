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
}

public static class FoodTypeExtensions
{
    public static Texture2D GetFoodTypeTexture(this FoodType foodType)
    {
        return foodType switch
        {
            FoodType.White => GD.Load<Texture2D>("Assets/white_food.png"),
            FoodType.Green => GD.Load<Texture2D>("Assets/green_food.png"),
            FoodType.Blue => GD.Load<Texture2D>("Assets/blue_food.png"),
            FoodType.Red => GD.Load<Texture2D>("Assets/red_food.png"),
            FoodType.Yellow => GD.Load<Texture2D>("Assets/yellow_food.png"),
            FoodType.Purple => GD.Load<Texture2D>("Assets/purple_food.png"),
            FoodType.Pink => GD.Load<Texture2D>("Assets/pink_food.png"),
            _ => throw new System.NotImplementedException(),
        };
    }
}