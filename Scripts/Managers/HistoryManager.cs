using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class HistoryManager
{
    private record HistoryMove(FoodType FoodEaten, bool isLastFood, Vector2 FoodPosition, Vector2I FoodBoardStatePositionId, Eater Eater, Vector2 EaterPosition, Vector2I EaterBoardStatePositionId);

    public static HistoryManager Instance { get; } = new HistoryManager();

    private Stack<HistoryMove> _moves = new();

    public int MoveCount => _moves.Count;

    public void ResetHistory() => _moves.Clear();

    public void AddMove(Food food, Eater eater, Vector2 eaterPosition)
    {
        _moves.Push(new HistoryMove(food.FoodType, food.IsLast, food.GlobalPosition, food.BoardStatePositionId, eater, eaterPosition, eater.BoardStatePositionId));
    }

    public void UndoMove()
    {
        if (_moves.Count > 0 && ActionManager.IsPlayerActionAvailable())
        {
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