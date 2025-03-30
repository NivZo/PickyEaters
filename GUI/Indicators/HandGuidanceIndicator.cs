using Godot;
using System;

public partial class HandGuidanceIndicator : Node2D
{
    private const string SCENE_PATH = "res://GUI/Indicators/HandGuidanceIndicator.tscn";
    private AnimationPlayer _animationPlayer;
    private bool _isSwiping = false;
    private bool _isTraveling = false;
    private Timer _resetTimer;
    private Vector2 _from;
    private Vector2 _to;

    public override void _Ready()
    {
        base._Ready();
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("RESET");
        _resetTimer = new()
        {
            Autostart = false,
            WaitTime = 2f,
            OneShot = true,
        };
        _resetTimer.Timeout += () => {
            TweenUtils.Travel(this, _from).Finished += () => {
                _isTraveling = false;
                GlobalPosition = _from;
            };
        };
        AddChild(_resetTimer);
        
        Scale = new(.4f, .4f);
        TweenUtils.Pop(this, 1);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_isSwiping && !_isTraveling)
        {
            _isTraveling = true;
            var tween = TweenUtils.Travel(this, _to, 2f, Tween.TransitionType.Cubic);
            tween.Finished += () => _resetTimer.Start();
        }
    }


    public static HandGuidanceIndicator CreatePointing(Node parent, Vector2 position)
    {
        var hand = GD.Load<PackedScene>(SCENE_PATH).Instantiate<HandGuidanceIndicator>();
        hand.GlobalPosition = position;
        parent.AddChild(hand);
        hand._animationPlayer.Play("point");
        return hand;
    }

    public static HandGuidanceIndicator CreateSwiping(Node parent, Vector2 fromPosition, Vector2 toPosition)
    {
        var hand = GD.Load<PackedScene>(SCENE_PATH).Instantiate<HandGuidanceIndicator>();
        hand.GlobalPosition = fromPosition;
        hand._from = fromPosition;
        hand._to = toPosition;
        hand._isSwiping = true;
        parent.AddChild(hand);
        hand._animationPlayer.Play("RESET");
        return hand;
    }
}
