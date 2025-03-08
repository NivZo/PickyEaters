using System.Collections.Generic;
using Godot;

public class Direction
{
    public EaterType EaterType { get;}
    public DirectionName Name { get; }
    public Vector2I DirectionVector { get; }
    public RayCast2D RayCast { get; }
    public List<FoodType> ValidFoodTypes { get; }

    public Direction(EaterType eaterType, DirectionName directionName, Vector2I directionVector, RayCast2D rayCast, List<FoodType> validFoodTypes)
    {
        EaterType = eaterType;
        Name = directionName;
        DirectionVector = directionVector;
        RayCast = rayCast;
        ValidFoodTypes = validFoodTypes;
    }

    public bool CanMoveInDirection => GetFoodCollision() != null;
    public Food GetFoodCollision()
    {
        var collision = RayCast.GetCollider();
        if (collision is Node2D collisionNode
            && collisionNode.GetParent() is Food food
            && ValidFoodTypes.Contains(food.FoodType)
            && (!food.IsLast || LevelManager.CanEatLast(food.FoodType)))
        {
            return food;
        }

        return null;
    }

    public enum DirectionName
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }
}