using Godot;

public partial class PlayScreen : Node
{
    public override void _Ready()
    {
        base._Ready();

        var gameLayer = GetNode<CanvasLayer>("GameLayer");
        LevelManager.Setup(gameLayer);
        LevelManager.LoadLevel(LevelManager.CurrentLevelId);
    }
}
