using Godot;

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
            TierColor.Easy => new("7390ea"),
            TierColor.Medium => new("fec851"),
            TierColor.Hard => new("d85058"),
            TierColor.Expert => new("ff7dbb"),
            TierColor.Genius => new("7b51cf"),
            TierColor.Super => new("944d12"),
            TierColor.Master => new("2c2c2c"),
            _ => new()
        };
    }
}