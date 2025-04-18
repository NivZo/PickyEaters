using Godot;

public partial class EaterCollectionShowcase : EaterShowcase
{
    public override void _Ready()
    {
        base._Ready();

        var resource = Display.EaterFace.GetEaterResource();
        var name = SaveManager.ActiveSave.UnlockedFaces.Contains(resource.EaterFace) ? resource.EaterName.ToUpperInvariant() : "???";

        GetNode<RichTextLabel>("EaterName").Text = TextUtils.WaveString($"\n{name}", frequency: 4);
        GetNode<RichTextLabel>("EaterRarity").Text = TextUtils.WaveString($"\n{resource.EaterRarity}", frequency: 4);
        GetNode<Sprite2D>("RarityBadge").Modulate = resource.EaterRarity.GetRarityColor();
    }
}
