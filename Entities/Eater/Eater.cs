using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Eater : Node2D
{
    [Export] public EaterType EaterType { get; set; }
    [Export] public Godot.Collections.Array<FoodType> ValidFoodTypes { get; set; }
    private DragSelectComponent<Eater> _dragSelectComponent;
    public TargetPositionComponent TargetPositionComponent;
    
    private CollisionObject2D _collider;
    private AudioStreamPlayer _audioStreamPlayer;
    private List<Direction> _directions;
    private Vector2 _clickPositionAnchor;

    private Sprite2D _body;
    private Sprite2D _face;
    private bool _isTakingAction = false;


    public override void _Ready()
    {
        base._Ready();

        _face = GetNode<Sprite2D>("Face");
        _body = GetNode<Sprite2D>("Body");
        _body.Texture = EaterType.GetEaterTypeBodyTexture();

        _collider = GetNode<Area2D>("Area2D");
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        _dragSelectComponent = new(_collider, ActionManager.Instance.IsActionAvailable);
        _dragSelectComponent.Select += OnSelect;
        _dragSelectComponent.Deselect += OnDeselect;
        TargetPositionComponent = new(this);
        TargetPositionComponent.SetPinPosition();

        _directions = new()
        {
            new(EaterType, Direction.DirectionName.Up, Vector2I.Up, GetNode<RayCast2D>("MoveRayCasts/Up"), ValidFoodTypes.ToList()),
            new(EaterType, Direction.DirectionName.Down, Vector2I.Down, GetNode<RayCast2D>("MoveRayCasts/Down"), ValidFoodTypes.ToList()),
            new(EaterType, Direction.DirectionName.Left, Vector2I.Left, GetNode<RayCast2D>("MoveRayCasts/Left"), ValidFoodTypes.ToList()),
            new(EaterType, Direction.DirectionName.Right, Vector2I.Right, GetNode<RayCast2D>("MoveRayCasts/Right"), ValidFoodTypes.ToList()),
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_dragSelectComponent.IsSelected)
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

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (_dragSelectComponent.IsSelected && @event is InputEventMouseButton inputEventMouseButton
            && inputEventMouseButton.ButtonIndex == MouseButton.Left)
        {
            if (inputEventMouseButton.IsReleased())
            {
                _dragSelectComponent.ManualDeselection();
            }
        }
    }


    private void OnSelect()
    {
        _clickPositionAnchor = GlobalPosition;
        _face.Texture = FaceTypeExtensions.GetFaceTypeTexture(FaceType.Excited);
        TweenUtils.Pop(this, 1.3f);
        TweenUtils.BoldOutline(_body, 8, 12);
        ZIndex = 1;

        AudioManager.Instance.PlayAudio(AudioType.SelectEater);
    }

    private void OnDeselect()
    {
        _directions.ForEach(direction => 
        {
            var collision = direction.GetFoodCollision();
            TweenUtils.Pop(collision, 1);
            TweenUtils.BoldOutline(collision?.Sprite, 8, 12);

        });
        _face.Texture = FaceTypeExtensions.GetFaceTypeTexture(FaceType.Smile);
        TargetPositionComponent.ResetNudge();
        TweenUtils.Pop(this, 1);
        TweenUtils.BoldOutline(_body, 4, 8);
        ZIndex = 0;

        var chosenDirection = _directions.FirstOrDefault(direction => direction.Name == GetCurrentDirection() && direction.CanMoveInDirection, null);
        if (chosenDirection != null)
        {
            var food = chosenDirection.GetFoodCollision();
            var currPos = TargetPositionComponent.NudgelessTargetPosition;
            ActionManager.Instance.StartAction(this, () => {
                HistoryManager.Instance.AddMove(food.FoodType, food.IsLast, food.GlobalPosition, this, currPos);
                food.QueueFree();
                AudioManager.Instance.PlayAudio(AudioType.FoodConsumed);
            });
            TargetPositionComponent.SetPinPosition(food.GlobalPosition);
        }
        else
        {
            AudioManager.Instance.PlayAudio(AudioType.DeselectEater);
        }
    }

    private void ProcessMovement()
    {
        if (GlobalPosition != TargetPositionComponent.TargetPosition)
        {
            TweenUtils.Travel(this, TargetPositionComponent.TargetPosition);
        }
        if (GlobalPosition.DistanceSquaredTo(TargetPositionComponent.TargetPosition) < 2500 && ActionManager.Instance.Actor == this)
        {
            ActionManager.Instance.FinishAction();
        }
    }

    private Direction.DirectionName GetCurrentDirection()
    {
        var dist = GetGlobalMousePosition() - _clickPositionAnchor;
        if (dist.LengthSquared() > 2500)
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
