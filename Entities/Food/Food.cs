using System;
using Godot;

[Tool]
public partial class Food : Node2D
{
    [Export] public FoodType FoodType { get; set; }
    [Export] public bool IsLast { get; set; } = false;
    [Export] public Vector2I BoardStatePositionId;
    public Sprite2D Sprite { get; set; }
    private AnimationPlayer _animationPlayer; 

    public override void _Ready()
    {
        base._Ready();

        Sprite = GetNode<Sprite2D>("Sprite2D");

        Sprite.Texture = FoodType.GetFoodTypeTexture();

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        if (IsLast)
        {
            Sprite.GetNode<Sprite2D>("LastFoodIndicator").Visible = true;
        }

        StartIdleAnimation();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Sprite.Texture == null)
        {
            Sprite.Texture = FoodType.GetFoodTypeTexture();
        }
    }

    public void StartIdleAnimation()
    {
        var rnd = new Random();
        _animationPlayer.Play("float");
        var offset = rnd.NextDouble() * _animationPlayer.CurrentAnimation.Length;
        _animationPlayer.Advance(offset);
    }
}
