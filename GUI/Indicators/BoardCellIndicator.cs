using System;
using System.Linq;
using Godot;

public partial class BoardCellIndicator : Node2D
{
    private Vector2I _boardStatePositionId;
    private ColorRect _cell;
    private readonly static PackedScene _scene = GD.Load<PackedScene>("res://GUI/Indicators/BoardCellIndicator.tscn");
    private readonly static Color _originalColor = NamedColor.TransparentGray.GetColor();
    private readonly static Color _consumedColor = NamedColor.White.GetColor();

    private bool _isConsumed => _cell.Color == _consumedColor;

    public override void _Ready()
    {
        base._Ready();

        if (!_isConsumed)
        {
            EventManager.MoveSelectionStarted += HandleSelectionStarted;
            EventManager.MoveSelectionCancelled += HandleSelectionCancelled;
            EventManager.MovePerformed += HandleMovePerformed;
            EventManager.MoveUndone += HandleSelectionUndone;
        }

        Scale = Vector2.Zero;
        TweenUtils.Pop(this, 1, transitionType: Tween.TransitionType.Bounce);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DisconnectSignals();
    }

    public static BoardCellIndicator Create(Vector2 globalPosition, Vector2I boardStatePositionId, bool isConsumed = false)
    {
        var ind = _scene.Instantiate<BoardCellIndicator>();
        ind.GlobalPosition = globalPosition;
        ind._boardStatePositionId = boardStatePositionId;
        ind._cell = ind.GetNode<ColorRect>("Cell");
        if (isConsumed)
        {
            ind.Highlight(_consumedColor);
        }

        return ind;
    }

    private void DisconnectSignals()
    {
        EventManager.MoveSelectionStarted -= HandleSelectionStarted;
        EventManager.MoveSelectionCancelled -= HandleSelectionCancelled;
        EventManager.MovePerformed -= HandleMovePerformed;
        EventManager.MoveUndone -= HandleSelectionUndone;
    }

    private void HandleSelectionStarted(Vector2I eaterPosId, Vector2I possibleFoodPosId, bool isCurrentlySelected)
    {
        if (_boardStatePositionId == possibleFoodPosId)
        {
            var color = LevelManager.Level.GetEaters().FirstOrDefault(eater => eater.BoardStatePositionId == eaterPosId)?.EaterType.GetNamedColor().GetColor() ?? NamedColor.White.GetColor();
            Highlight(color with { A = isCurrentlySelected ? 1 : .5f });
        }
    }

    private void HandleSelectionCancelled(Vector2I eaterPosId)
    {
        Highlight(_originalColor);
    }

    private void HandleSelectionUndone(Vector2I eaterPosId, Vector2I foodPosId, FoodType foodType, bool isLast)
    {
        if (foodPosId == _boardStatePositionId)
        {
            Highlight(_originalColor, true);
        }
    }

    private void HandleMovePerformed(Vector2I eaterPosId, Vector2I foodPosId, FoodType foodType, bool isLast, bool isHint)
    {
        if (foodPosId == _boardStatePositionId)
        {
            Highlight(_consumedColor);
        }
        else
        {
            Highlight(_originalColor);
        }
    }

    private void Highlight(Color color, bool overrideIsConsumed = false)
    {
        if (overrideIsConsumed || !_isConsumed)
        {
            _cell.Color = color;
        }
    }
}
