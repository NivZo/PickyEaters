using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class HistoryManager
{
    record HistoryMove(FoodType FoodEaten, bool isLastFood, Vector2 FoodPosition, Eater Eater, Vector2 EaterPosition);

    public static HistoryManager Instance { get; } = new HistoryManager();

    private Stack<HistoryMove> _moves = new();

    public int MoveCount => _moves.Count;

    public void ResetHistory() => _moves.Clear();

    public void AddMove(FoodType foodEaten, bool isLastFood, Vector2 foodPosition, Eater eater, Vector2 eaterPosition)
    {
        _moves.Push(new HistoryMove(foodEaten, isLastFood, foodPosition, eater, eaterPosition));
    }

    public void UndoMove()
    {
        if (_moves.Count > 0 && ActionManager.Instance.IsActionAvailable())
        {
            var lastMove = _moves.Pop();

            if (lastMove != null)
            {
                ActionManager.Instance.StartAction(lastMove.Eater, () => {
                    var food = GD.Load<PackedScene>("res://Entities/Food/Food.tscn").Instantiate<Food>();
                    food.GlobalPosition = lastMove.FoodPosition;
                    food.FoodType = lastMove.FoodEaten;
                    food.IsLast = lastMove.isLastFood;
                    food.Scale = Vector2.Zero;
                    LevelManager.Instance.Level.Food.AddChild(food);
                    TweenUtils.Pop(food, 1);
                });

                lastMove.Eater.Scale = new(0.5f, 0.5f);
                TweenUtils.Pop(lastMove.Eater, 1);
                lastMove.Eater.TargetPositionComponent.SetPinPosition(lastMove.EaterPosition);
            }
        }
    }
}