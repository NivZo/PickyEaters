using System;
using Godot;

[Tool]
public partial class Wall : Node2D
{
    [Export] public Vector2I BoardStatePositionId;
    public Sprite2D Sprite { get; set; }

    public override void _Ready()
    {
        base._Ready();

        Sprite = GetNode<Sprite2D>("Sprite2D");
    }
}
