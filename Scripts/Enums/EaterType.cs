using Godot;

public enum EaterType
{
    Green,
    Blue,
    Red,
    Yellow,
    Purple,
    Pink,
    Hidden,

}

public static class EaterTypeExtensions
{
    public static Texture2D GetEaterTypeBodyTexture(this EaterType eaterType)
    {
        return eaterType switch
        {
            EaterType.Hidden => GD.Load<Texture2D>("Assets/Entities/eater_hidden.png"),
            EaterType.Green => GD.Load<Texture2D>("Assets/Entities/green_eater_body.png"),
            EaterType.Blue => GD.Load<Texture2D>("Assets/Entities/blue_eater_body.png"),
            EaterType.Red => GD.Load<Texture2D>("Assets/Entities/red_eater_body.png"),
            EaterType.Yellow => GD.Load<Texture2D>("Assets/Entities/yellow_eater_body.png"),
            EaterType.Purple => GD.Load<Texture2D>("Assets/Entities/purple_eater_body.png"),
            EaterType.Pink => GD.Load<Texture2D>("Assets/Entities/pink_eater_body.png"),
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