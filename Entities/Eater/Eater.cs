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
    public EaterDisplay Display;
    public CpuParticles2D EatParticlesEmitter;

    private SelectComponent<Eater> _selectComponent;
    private AudioStreamPlayer _audioStreamPlayer;
    private List<Direction> _directions;
    private Direction.DirectionName _currentSelectedDirection = Direction.DirectionName.None;
    private Vector2 _clickPositionAnchor;

    private bool _isTakingAction = false;


    public override void _Ready()
    {
        base._Ready();
        
        EaterFace = EaterFace == EaterFace.SmileBasic ? EnumUtils.GetRandomValueOutOf(SaveManager.ActiveSave.UnlockedFaces.ToList()) : EaterFace;
        Display = GetNode<EaterDisplay>("EaterDisplay");
        Display.EaterFace = EaterFace;
        Display.EaterType = EaterType;
        Display.Setup();
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        EatParticlesEmitter = GetNode<CpuParticles2D>("EatParticles");
        _selectComponent = Display.SelectComponent;
        _selectComponent.Select += OnSelect;
        _selectComponent.Deselect += OnDeselect;
        TargetPositionComponent = new(this);
        TargetPositionComponent.SetPinPosition();

        _directions = new()
        {
            new(Display.EaterType, Direction.DirectionName.Up, Vector2I.Up, Display.GetNode<RayCast2D>("MoveRayCasts/Up"), ValidFoodTypes.ToList()),
            new(Display.EaterType, Direction.DirectionName.Down, Vector2I.Down, Display.GetNode<RayCast2D>("MoveRayCasts/Down"), ValidFoodTypes.ToList()),
            new(Display.EaterType, Direction.DirectionName.Left, Vector2I.Left, Display.GetNode<RayCast2D>("MoveRayCasts/Left"), ValidFoodTypes.ToList()),
            new(Display.EaterType, Direction.DirectionName.Right, Vector2I.Right, Display.GetNode<RayCast2D>("MoveRayCasts/Right"), ValidFoodTypes.ToList()),
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_selectComponent.IsSelected)
        {
            var currentDirection = GetCurrentDirection();

            if (_currentSelectedDirection != currentDirection)
            {
                _currentSelectedDirection = currentDirection;
                EmitSelectionStartSignals();
                var shouldResetNudge = true;

                var chosenDirection = _directions.FirstOrDefault(direction => direction.Name == _currentSelectedDirection && direction.CanMoveInDirection, null);
                if (chosenDirection != null)
                {
                    TargetPositionComponent.Nudge(chosenDirection.DirectionVector * 60);
                    shouldResetNudge = false;
                }

                if (shouldResetNudge)
                {
                    TargetPositionComponent.ResetNudge();
                }
            }
        }
    
        ProcessMovement();
    }

    public void PerformMove(Food food, bool isHint)
    {
        var currPos = TargetPositionComponent.NudgelessTargetPosition;
        ActionManager.StartPlayerAction(this, () => {
            if (food.IsLast)
            {
                EatParticlesEmitter.OneShot = false;
                Display.ToggleFinished(true);
            }

            EatParticlesEmitter.Texture = food.FoodType.GetFoodTypeTexture();
            EatParticlesEmitter.Emitting = true;
            MainCamera.ApplyShake();

            HistoryManager.AddMove(food, this, currPos);
            SignalProvider.Emit(SignalProvider.SignalName.MovePerformed, BoardStatePositionId, food.BoardStatePositionId, Variant.From(food.FoodType), food.IsLast, isHint);        
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

        EmitSelectionStartSignals();
    }

    private void OnDeselect()
    {
        ZIndex = 0;
        TargetPositionComponent.ResetNudge();

        var chosenDirection = _directions.FirstOrDefault(direction => direction.Name == _currentSelectedDirection && direction.CanMoveInDirection, null);
        if (chosenDirection != null)
        {
            var food = chosenDirection.GetFoodCollision();
            PerformMove(food, false);
        }
        else
        {
            SignalProvider.Emit(SignalProvider.SignalName.MoveSelectionCancelled, BoardStatePositionId);
        }
    }

    private void EmitSelectionStartSignals()
    {
        foreach (var direction in _directions.Where(dir => dir.CanMoveInDirection))
        {
            var food = direction.GetFoodCollision();
            if (food != null)
            {
                SignalProvider.Emit(SignalProvider.SignalName.MoveSelectionStarted, BoardStatePositionId, food.BoardStatePositionId, direction.Name == _currentSelectedDirection);
            }
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
