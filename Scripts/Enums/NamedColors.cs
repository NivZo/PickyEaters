using Godot;

public enum NamedColor
{
    Blank,
    Green,
    Blue,
    Red,
    Yellow,
    Purple,
    Pink,
    Brown,
    Black,
    White,
    Gray,
    Cyan,
    Orange,
    TransparentGray,
}

public static class EaterColorExtensions
{
    public static Color GetColor(this NamedColor color)
    {
        return color switch
        {
            NamedColor.Blank => new("ffffff"),
            NamedColor.Green => new("59b25d"),
            NamedColor.Blue => new("6c77e9"),
            NamedColor.Yellow => new("fec851"),
            NamedColor.Red => new("ff2e45"),
            NamedColor.Pink => new("ff7dbb"),
            NamedColor.Purple => new("7b51cf"),
            NamedColor.Brown => new("944d12"),
            NamedColor.Black => new("2c2c2c"),
            NamedColor.White => new("bcbcbc3c"),
            NamedColor.Gray => new("bcc7c9"),
            NamedColor.Cyan => new("87bfc6"),
            NamedColor.Orange => new("ffaf00"),
            NamedColor.TransparentGray => new("7e7e7e3c"),
            _ => new()
        };
    }
}

public enum TierColor
{
    Easy,
    Medium,
    Hard,
    Expert,
    Genius,
    Super,
    Master
}

public static class TierColorExtensions
{
    public static Color GetColor(this TierColor color)
    {
        return color switch
        {
            TierColor.Easy => NamedColor.Blue.GetColor(),
            TierColor.Medium => NamedColor.Yellow.GetColor(),
            TierColor.Hard => NamedColor.Red.GetColor(),
            TierColor.Expert => NamedColor.Pink.GetColor(),
            TierColor.Genius => NamedColor.Purple.GetColor(),
            TierColor.Super => NamedColor.Brown.GetColor(),
            TierColor.Master => NamedColor.Black.GetColor(),
            _ => new()
        };
    }
}