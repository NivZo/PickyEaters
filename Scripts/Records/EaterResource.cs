using Godot;

[GlobalClass]
public partial class EaterResource : Resource
{
    [Export] public EaterFace EaterFace { get; set; } = EaterFace.Hidden;
    [Export] public string EaterName { get; set; } = string.Empty;
    [Export] public Rarity EaterRarity { get; set; } = Rarity.Common;
    [Export] public Texture2D PassiveTexture { get; set; }
    [Export] public Texture2D ActiveTexture { get; set; }
}