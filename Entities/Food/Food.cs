using System;
using System.Linq;
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

        EventManager.MoveSelectionStarted += HandleSelectionStarted;
        EventManager.MovePerformed += HandleSelectionEnded;
        EventManager.MoveSelectionCancelled += HandleSelectionCancelled;

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

    public override void _ExitTree()
    {
        base._ExitTree();

        EventManager.MoveSelectionStarted -= HandleSelectionStarted;
        EventManager.MovePerformed -= HandleSelectionEnded;
        EventManager.MoveSelectionCancelled -= HandleSelectionCancelled;
    }

    private void StartIdleAnimation()
    {
        var rnd = new Random();
        _animationPlayer.Play("float");
        var offset = rnd.NextDouble() * _animationPlayer.CurrentAnimation.Length;
        _animationPlayer.Advance(offset);
    }

    private void HandleSelectionStarted(Vector2I EaterPosId, Vector2I FoodPosId, bool IsCurrentlySelected)
    {
        if (FoodPosId == BoardStatePositionId)
        {
            if (IsCurrentlySelected)
            {
                TweenUtils.Pop(this, 1.4f);
                TweenUtils.BoldOutline(Sprite, 16, 20);
            }
            else
            {
                TweenUtils.Pop(this, 1.2f);
                TweenUtils.BoldOutline(Sprite, 12, 16);
            }
        }
    }

    private void HandleSelectionEnded(Vector2I EaterPosId, Vector2I FoodPosId, FoodType FoodType, bool IsLast, bool IsHint)
    {
        if (FoodPosId != BoardStatePositionId)
        {
            TweenUtils.Pop(this, 1);
            TweenUtils.BoldOutline(Sprite, 8, 12);
        }
    }

    private void HandleSelectionCancelled(Vector2I EaterPosId)
    {
        TweenUtils.Pop(this, 1);
        TweenUtils.BoldOutline(Sprite, 8, 12);
    }
}
