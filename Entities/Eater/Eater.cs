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
    private static DateTime _lastEatTimestamp = DateTime.Now;
    private static int _eatStreak = 0;
    private const int _maxStreakPause = 2;

    private bool _isTakingAction = false;


    public override void _Ready()
    {
        base._Ready();
        
        EaterFace = EaterFace == EaterFace.SmileBasic ? EnumUtils.GetRandomValueOutOf(SaveManager.ActiveSave.UnlockedFaces.ToList()) : EaterFace;
        Display = GetNode<EaterDisplay>("EaterDisplay");
        Display.EaterFace = EaterFace;
        Display.EaterType = EaterType;
        Display.Setup();
        Display.EnableSelectComponent();
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        EatParticlesEmitter = GetNode<CpuParticles2D>("EatParticles");
        _selectComponent = Display.SelectComponent;
        _selectComponent.Select += OnSelect;
        _selectComponent.Deselect += OnDeselect;
        TargetPositionComponent = new(this);
        TargetPositionComponent.SetPinPosition();

        _directions = new()
        {
            new(Display.EaterType, Direction.DirectionName.Up, Vector2I.Up, GetNode<RayCast2D>("MoveRayCasts/Up"), ValidFoodTypes.ToList()),
            new(Display.EaterType, Direction.DirectionName.Down, Vector2I.Down, GetNode<RayCast2D>("MoveRayCasts/Down"), ValidFoodTypes.ToList()),
            new(Display.EaterType, Direction.DirectionName.Left, Vector2I.Left, GetNode<RayCast2D>("MoveRayCasts/Left"), ValidFoodTypes.ToList()),
            new(Display.EaterType, Direction.DirectionName.Right, Vector2I.Right, GetNode<RayCast2D>("MoveRayCasts/Right"), ValidFoodTypes.ToList()),
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
        _eatStreak = DateTime.Now - _lastEatTimestamp < TimeSpan.FromSeconds(_maxStreakPause) ? _eatStreak + 1 : 0;
        _lastEatTimestamp = DateTime.Now;

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
            var eaterPosId = BoardStatePositionId;
            BoardStatePositionId = food.BoardStatePositionId;
            EventManager.InvokeMovePerformed(eaterPosId, food.BoardStatePositionId, food.FoodType, food.IsLast, isHint);        
            food.QueueFree();
            AudioManager.PlayAudio(AudioType.FoodConsumed, 1 + _eatStreak * 0.1f);
        });

        TargetPositionComponent.SetPinPosition(food.GlobalPosition);
    }

    private void OnSelect()
    {
        _clickPositionAnchor = GetGlobalMousePosition();
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
            EventManager.InvokeMoveSelectionCancelled(BoardStatePositionId);
        }
    }

    private void EmitSelectionStartSignals()
    {
        foreach (var direction in _directions.Where(dir => dir.CanMoveInDirection))
        {
            var food = direction.GetFoodCollision();
            if (food != null)
            {
                EventManager.InvokeMoveSelectionStarted(BoardStatePositionId, food.BoardStatePositionId, direction.Name == _currentSelectedDirection);
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
