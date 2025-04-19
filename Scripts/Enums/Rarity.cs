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
            Rarity.Common => NamedColor.Blank.GetColor(),
            Rarity.Rare => NamedColor.Blue.GetColor(),
            Rarity.Epic => NamedColor.Purple.GetColor(),
            Rarity.Legendary => NamedColor.Yellow.GetColor(),
            _ => new(),
        };
    }
}