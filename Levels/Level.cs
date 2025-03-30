using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Level : Node
{
    public Node Food;
    public Node Eaters;
    private Node _indicators;
    private Dictionary<Vector2I, BoardCellIndicator> _boardCellIndicatorMapping = new();
    
    public override void _Ready()
    {
        base._Ready();

        Food = GetNode<Node>("Food");
        Eaters = GetNode<Node>("Eaters");

        _indicators = new Node();
        AddChild(_indicators);

        foreach (var food in GetFood())
        {
            var ind = BoardCellIndicator.Create(food.GlobalPosition, food.BoardStatePositionId);
            _indicators.AddChild(ind);
            _boardCellIndicatorMapping.Add(food.BoardStatePositionId, ind);

        }

        foreach (var eater in GetEaters())
        {
            var ind = BoardCellIndicator.Create(eater.GlobalPosition, eater.BoardStatePositionId, true);
            _indicators.AddChild(ind);
            _boardCellIndicatorMapping.Add(eater.BoardStatePositionId, ind);

        }

        EventManager.MovePerformed += HandleMove;
        EventManager.InvokeLevelReset();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventManager.MovePerformed -= HandleMove;
    }

    public List<Eater> GetEaters() => Eaters
        .GetChildren()
        .Where(child => child is Eater)
        .Select(eater => eater as Eater).ToList();

    
    public List<Food> GetFood() => Food
        .GetChildren()
        .Where(child => child is Food)
        .Select(eater => eater as Food).ToList();
    
    public List<FoodType> GetUnfinishedFoodTypes() => GetFood().Where(food => food.IsLast).Select(food => food.FoodType).ToList();
    
    public Vector2 BoardPositionIdToGlobalPosition(Vector2 posId)
    {
        foreach (var eater in GetEaters())
        {
            if (eater.BoardStatePositionId == posId)
            {
                return eater.TargetPositionComponent.TargetPosition;
            }
        }

        foreach (var food in GetFood())
        {
            if (food.BoardStatePositionId == posId)
            {
                return food.GlobalPosition;
            }
        }

        return Vector2.Zero;
    }

    private void HandleMove(Vector2I EaterPosId, Vector2I FoodPosId, FoodType FoodType, bool IsLast, bool IsHint)
    {
        if (IsHint)
        {
            HintManager.ActivateHint();
        }
        else
        {
            HintManager.HandleNonHintMove();
        }

        var unfinished = LevelManager.Level.GetUnfinishedFoodTypes();
        if (unfinished.Count == 1 && unfinished.FirstOrDefault() == FoodType && IsLast)
        {
            ModalManager.OpenVictoryModal();
        }
    }
}
