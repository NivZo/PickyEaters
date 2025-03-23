using Godot;

public enum EaterType
{
    Green,
    Blue,
    Red,
    Yellow,
    Purple,
    Pink,
    Brown,
    Hidden,

}

public static class EaterTypeExtensions
{
    public static NamedColor GetNamedColor(this EaterType eaterType)
    {
        return eaterType switch
        {
            EaterType.Green => NamedColor.Green,
            EaterType.Blue => NamedColor.Blue,
            EaterType.Red => NamedColor.Red,
            EaterType.Yellow => NamedColor.Yellow,
            EaterType.Purple => NamedColor.Purple,
            EaterType.Pink => NamedColor.Pink,
            EaterType.Brown => NamedColor.Brown,
            EaterType.Hidden => NamedColor.Black,
            _ => NamedColor.White,
        };
    }

    public static Texture2D GetEaterTypeBodyTexture(this EaterType eaterType)
    {
        return eaterType switch
        {
            EaterType.Hidden => GD.Load<Texture2D>("Assets/Entities/eater_hidden.png"),
            _ => GD.Load<Texture2D>($"Assets/Entities/{eaterType.ToString().ToLowerInvariant()}_eater_body.png"),
        };
    }

    public static Texture2D GetEaterTypeHandThumbTexture(this EaterType eaterType)
    {
        return eaterType switch
        {
            EaterType.Hidden => null,
            _ => GD.Load<Texture2D>($"Assets/Entities/{eaterType.ToString().ToLowerInvariant()}_hand_thumb.png"),
        };
    }

    // public static Color GetEaterTypeBodyModulate(this EaterType eaterType)
    // {
    //     return eaterType switch
    //     {
    //         EaterType.Green => new Color("98fa9d"),
    //         EaterType.Blue => new Color("58b4ef"),
    //         EaterType.Red => new Color("ff7991"),
    //         EaterType.Yellow => new Color("ffd54f"),
    //         EaterType.Purple => new Color("ab00f7"),
    //         EaterType.Pink => new Color("ff7dff"),
    //         _ => throw new System.NotImplementedException(),
    //     };
    // }
}