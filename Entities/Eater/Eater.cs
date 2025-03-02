using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Eater : Node2D
{
    [Export] public EaterType EaterType { get; set; }
    [Export] public EaterFace EaterFace { get; set; } = EaterFace.SmileBasic;
    [Export] public Vector2I BoardStatePositionId;
    [Export] public Godot.Collections.Array<FoodType> ValidFoodTypes { get; set; }
    public TargetPositionComponent TargetPositionComponent;
    private SelectComponent<Eater> _selectComponent;

    private EaterDisplay _display;    
    private AudioStreamPlayer _audioStreamPlayer;
    private List<Direction> _directions;
    private Vector2 _clickPositionAnchor;

    private bool _isTakingAction = false;


    public override void _Ready()
    {
        base._Ready();
        
        EaterFace = EaterFace == EaterFace.SmileBasic ? EnumUtils.GetRandomValueOutOf(SaveManager.ActiveSave.UnlockedFaces.ToList()) : EaterFace;
        _display = GetNode<EaterDisplay>("EaterDisplay");
        _display.EaterFace = EaterFace;
        _display.EaterType = EaterType;
        _display.Setup();
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        _selectComponent = _display.SelectComponent;
        _selectComponent.Select += OnSelect;
        _selectComponent.Deselect += OnDeselect;
        TargetPositionComponent = new(this);
        TargetPositionComponent.SetPinPosition();

        _directions = new()
        {
            new(_display.EaterType, Direction.DirectionName.Up, Vector2I.Up, _display.GetNode<RayCast2D>("MoveRayCasts/Up"), ValidFoodTypes.ToList()),
            new(_display.EaterType, Direction.DirectionName.Down, Vector2I.Down, _display.GetNode<RayCast2D>("MoveRayCasts/Down"), ValidFoodTypes.ToList()),
            new(_display.EaterType, Direction.DirectionName.Left, Vector2I.Left, _display.GetNode<RayCast2D>("MoveRayCasts/Left"), ValidFoodTypes.ToList()),
            new(_display.EaterType, Direction.DirectionName.Right, Vector2I.Right, _display.GetNode<RayCast2D>("MoveRayCasts/Right"), ValidFoodTypes.ToList()),
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_selectComponent.IsSelected)
        {
            var currentDirection = GetCurrentDirection();
            var shouldResetNudge = true;

            foreach (var direction in _directions)
            {
                var collision = direction.GetFoodCollision();
                var canMoveToCollision = direction.CanMoveInDirection && collision != null; 
                if (direction.Name == currentDirection && canMoveToCollision)
                {
                    TweenUtils.Pop(collision, 1.5f);
                    TweenUtils.BoldOutline(collision?.Sprite, 16, 20);
                    TargetPositionComponent.Nudge(direction.DirectionVector * 40);
                    shouldResetNudge = false;
                }
                else if (direction.Name != currentDirection && canMoveToCollision)
                {
                    TweenUtils.Pop(collision, 1.3f);
                    TweenUtils.BoldOutline(collision?.Sprite, 14, 18);
                }
                else
                {
                    TweenUtils.Pop(collision, 1);
                    TweenUtils.BoldOutline(collision?.Sprite, 8, 12);
                }
            }

            if (shouldResetNudge)
            {
                TargetPositionComponent.ResetNudge();
            }
        }
    
        ProcessMovement();
    }

    public void PerformMove(Food food, bool isHint)
    {
        var currPos = TargetPositionComponent.NudgelessTargetPosition;
        ActionManager.StartPlayerAction(this, () => {
            HistoryManager.Instance.AddMove(food, this, currPos);
            SignalProvider.Emit(SignalProvider.SignalName.MovePerformed, BoardStatePositionId, food.BoardStatePositionId, isHint);
            BoardStatePositionId = food.BoardStatePositionId;
            food.QueueFree();
            AudioManager.PlayAudio(AudioType.FoodConsumed);
        });
        TargetPositionComponent.SetPinPosition(food.GlobalPosition);
    }

    private void OnSelect()
    {
        _clickPositionAnchor = GlobalPosition;
        ZIndex = 1;
    }

    private void OnDeselect()
    {
        ZIndex = 0;
        _directions.ForEach(direction => 
        {
            var collision = direction.GetFoodCollision();
            TweenUtils.Pop(collision, 1);
            TweenUtils.BoldOutline(collision?.Sprite, 8, 12);

        });
        TargetPositionComponent.ResetNudge();
        

        var chosenDirection = _directions.FirstOrDefault(direction => direction.Name == GetCurrentDirection() && direction.CanMoveInDirection, null);
        if (chosenDirection != null)
        {
            var food = chosenDirection.GetFoodCollision();
            PerformMove(food, false);
        }
    }

    private void ProcessMovement()
    {
        if (GlobalPosition != TargetPositionComponent.TargetPosition)
        {
            TweenUtils.Travel(this, TargetPositionComponent.TargetPosition);
        }
        if (GlobalPosition.DistanceSquaredTo(TargetPositionComponent.TargetPosition) < 2500 && ActionManager.Actor == this)
        {
            ActionManager.FinishPlayerAction();
        }
    }

    private Direction.DirectionName GetCurrentDirection()
    {
        var dist = GetGlobalMousePosition() - _clickPositionAnchor;
        if (dist.LengthSquared() > 5000)
        {
            return dist switch
            {
                { X: var x, Y: var y } when Math.Abs(x) < -y => Direction.DirectionName.Up,
                { X: var x, Y: var y } when Math.Abs(x) < y => Direction.DirectionName.Down,
                { X: var x, Y: var y } when -x < y && y < x => Direction.DirectionName.Right,
                { X: var x, Y: var y } when x < y && y < -x => Direction.DirectionName.Left,
                _ => Direction.DirectionName.None,
            };
        }
        
        return Direction.DirectionName.None;
    }
}
