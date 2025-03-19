using Godot;

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

public static class RarityExtensions
{
    public static Color GetRarityColor(this Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => new("ffffff"),
            Rarity.Rare => TierColor.Easy.GetColor(),
            Rarity.Epic => TierColor.Genius.GetColor(),
            Rarity.Legendary => TierColor.Medium.GetColor(),
            _ => new(),
        };
    }
}