using System.Collections.Generic;
using Godot;

public static class HistoryManager
{
    private record HistoryMove(FoodType FoodEaten, bool isLastFood, Vector2 FoodPosition, Vector2I FoodBoardStatePositionId, Eater Eater, Vector2 EaterPosition, Vector2I EaterBoardStatePositionId);

    private static Stack<HistoryMove> _moves = new();

    public static int UndoCount { get; private set; } = 10;

    public static int MoveCount => _moves.Count;

    public static void ResetHistory()
    {
        _moves.Clear();
        ResetUndos();
    }

    public static void ResetUndos()
    {
        UndoCount = 10;
    }

    public static void AddMove(Food food, Eater eater, Vector2 eaterPosition)
    {
        _moves.Push(new HistoryMove(food.FoodType, food.IsLast, food.GlobalPosition, food.BoardStatePositionId, eater, eaterPosition, eater.BoardStatePositionId));
    }

    public static void UndoMove()
    {
        if (UndoCount > 0 && _moves.Count > 0 && ActionManager.IsPlayerActionAvailable())
        {
            UndoCount -= 1;
            var lastMove = _moves.Pop();

            if (lastMove != null)
            {
                ActionManager.StartPlayerAction(lastMove.Eater, () => {
                    var food = GD.Load<PackedScene>("res://Entities/Food/Food.tscn").Instantiate<Food>();
                    food.GlobalPosition = lastMove.FoodPosition;
                    food.FoodType = lastMove.FoodEaten;
                    food.IsLast = lastMove.isLastFood;
                    food.BoardStatePositionId = lastMove.FoodBoardStatePositionId;
                    food.Scale = Vector2.Zero;
                    LevelManager.Level.Food.AddChild(food);
                    TweenUtils.Pop(food, 1);
                });

                lastMove.Eater.Scale = new(0.5f, 0.5f);
                TweenUtils.Pop(lastMove.Eater, 1);
                lastMove.Eater.TargetPositionComponent.SetPinPosition(lastMove.EaterPosition);
                lastMove.Eater.BoardStatePositionId = lastMove.EaterBoardStatePositionId;
                if (lastMove.isLastFood)
                {
                    lastMove.Eater.Display.ToggleFinished(false);
                    lastMove.Eater.EatParticlesEmitter.OneShot = true;
                    lastMove.Eater.EatParticlesEmitter.Emitting = false;
                }
            }
        }
    }
}