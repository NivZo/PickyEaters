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

        Sprite = GetNode<Sprite2D>("Food");

        Sprite.Texture = FoodType.GetFoodTypeTexture(IsLast);

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

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
            Sprite.Texture = FoodType.GetFoodTypeTexture(IsLast);
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
        if (IsLast)
        {
            _animationPlayer.Play("float_starfood");
        }
        else
        {
            var rnd = new Random();
            _animationPlayer.Play("float");
            var offset = rnd.NextDouble() * _animationPlayer.CurrentAnimation.Length;
            _animationPlayer.Advance(offset);
        }
    }

    private void HandleSelectionStarted(Vector2I EaterPosId, Vector2I FoodPosId, bool IsCurrentlySelected)
    {
        if (FoodPosId == BoardStatePositionId)
        {
            if (IsCurrentlySelected)
            {
                Scale = Vector2.One;
                TweenUtils.Pop(this, 1.4f);
                TweenUtils.BoldOutline(Sprite, 16, 20);
                Input.VibrateHandheld(100, (float)SaveManager.ActiveSave.ScreenShakeStrength * 0.05f);
            }
            else
            {
                Scale = new Vector2(.8f, .8f);
                TweenUtils.Pop(this, 1.1f);
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
