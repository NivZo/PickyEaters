using Godot;
using System;

public partial class HandGuidanceIndicator : Node2D
{
    [Export] public HandGuidanceIndicatorType GestureType;
    private const string SCENE_PATH = "res://GUI/Indicators/HandGuidanceIndicator.tscn";
    private AnimationPlayer _animationPlayer;
    private bool _isSwiping = false;
    private bool _isTraveling = false;
    private Timer _resetTimer;
    private Vector2 _from;
    private Vector2? _to;

    public enum HandGuidanceIndicatorType
    {
        Pointing,
        Swiping,
        Tapping,
    }

    public override void _Ready()
    {
        base._Ready();
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("RESET");

        if (_from == default)
        {
            _from = GlobalPosition;
        }

        switch (GestureType)
        {
            case HandGuidanceIndicatorType.Pointing:
                SetupPointing(_from);
                break;
            case HandGuidanceIndicatorType.Swiping:
                if (_to == null)
                    throw new ArgumentNullException(nameof(_to), "Swipe target must be provided for swiping.");
                SetupSwiping(_from, _to.Value);
                break;
            case HandGuidanceIndicatorType.Tapping:
                SetupTapping(_from);
                break;
        }

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

        if (_isSwiping && !_isTraveling && _to.HasValue)
        {
            _isTraveling = true;
            var tween = TweenUtils.Travel(this, _to.Value, 2f, Tween.TransitionType.Cubic);
            tween.Finished += () => _resetTimer.Start();
        }
    }

    public static HandGuidanceIndicator Create(HandGuidanceIndicatorType type, Node parent, Vector2 position, Vector2? swipeTarget = null)
    {
        var scene = GD.Load<PackedScene>(SCENE_PATH);
        var instance = scene.Instantiate<HandGuidanceIndicator>();
        instance.GestureType = type;
        instance._from = position;
        instance._to = swipeTarget;
        parent.AddChild(instance);
        return instance;
    }


    private void SetupPointing(Vector2 position)
    {
        GlobalPosition = position;
        _animationPlayer.Play("point");
    }

    private void SetupSwiping(Vector2 fromPosition, Vector2 toPosition)
    {
        GlobalPosition = fromPosition;
        _from = fromPosition;
        _to = toPosition;
        _isSwiping = true;
        _animationPlayer.Play("RESET");
    }

    private void SetupTapping(Vector2 position)
    {
        GlobalPosition = position;
        _animationPlayer.Play("tap");
    }
}
