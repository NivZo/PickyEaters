using Godot;

public enum EaterType
{
    Green,
    Blue,
    Red,
    Yellow,
    Purple,
    Pink,
}

public static class EaterTypeExtensions
{
    public static Texture2D GetEaterTypeBodyTexture(this EaterType eaterType)
    {
        return eaterType switch
        {
            EaterType.Green => GD.Load<Texture2D>("Assets/green_eater_body.png"),
            EaterType.Blue => GD.Load<Texture2D>("Assets/blue_eater_body.png"),
            EaterType.Red => GD.Load<Texture2D>("Assets/red_eater_body.png"),
            EaterType.Yellow => GD.Load<Texture2D>("Assets/yellow_eater_body.png"),
            EaterType.Purple => GD.Load<Texture2D>("Assets/purple_eater_body.png"),
            EaterType.Pink => GD.Load<Texture2D>("Assets/pink_eater_body.png"),
            _ => throw new System.NotImplementedException(),
        };
    }

    public static Color GetEaterTypeBodyModulate(this EaterType eaterType)
    {
        return eaterType switch
        {
            EaterType.Green => new Color("98fa9d"),
            EaterType.Blue => new Color("58b4ef"),
            EaterType.Red => new Color("ff7991"),
            EaterType.Yellow => new Color("ffd54f"),
            EaterType.Purple => new Color("ab00f7"),
            EaterType.Pink => new Color("ff7dff"),
            _ => throw new System.NotImplementedException(),
        };
    }
}