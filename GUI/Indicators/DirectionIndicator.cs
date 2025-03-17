using Godot;
using System;

public partial class DirectionIndicator : Node2D
{
    private Vector2 _from;
    private Vector2 _to;
    private TextureRect _rect;

    public static DirectionIndicator Create(Vector2 from, Vector2 to)
    {
        var ind = GD.Load<PackedScene>("res://GUI/Indicators/DirectionIndicator.tscn").Instantiate<DirectionIndicator>();
        ind._from = from;
        ind._to = to;

        ind._rect = ind.GetNode<TextureRect>("TextureRect");
        ind._rect.Size = new(ind._rect.Size.X, Math.Abs(ind._from.Y - ind._to.Y));

        return ind;
    }

    public override void _Ready()
    {
        base._Ready();
    }
}
