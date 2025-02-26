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
            FoodType.White => GD.Load<Texture2D>("Assets/Entities/white_food.png"),
            FoodType.Green => GD.Load<Texture2D>("Assets/Entities/green_food.png"),
            FoodType.Blue => GD.Load<Texture2D>("Assets/Entities/blue_food.png"),
            FoodType.Red => GD.Load<Texture2D>("Assets/Entities/red_food.png"),
            FoodType.Yellow => GD.Load<Texture2D>("Assets/Entities/yellow_food.png"),
            FoodType.Purple => GD.Load<Texture2D>("Assets/Entities/purple_food.png"),
            FoodType.Pink => GD.Load<Texture2D>("Assets/Entities/pink_food.png"),
            _ => throw new System.NotImplementedException(),
        };
    }
}