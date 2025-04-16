using System;
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

        var cutscenes = new List<CutsceneManager.CutsceneAction>();
        
        var foodNodes = GetFood();
        var eaterNodes = GetEaters();
        var cutsceneDelay = 1f / (foodNodes.Count + eaterNodes.Count);
        foreach (var food in foodNodes)
        {
            var ind = BoardCellIndicator.Create(food.GlobalPosition, food.BoardStatePositionId);
            _indicators.AddChild(ind);
            _boardCellIndicatorMapping.Add(food.BoardStatePositionId, ind);
            cutscenes.Add(new(CreatePopNodeCutsceneAction(food), cutsceneDelay));
        }

        foreach (var eater in eaterNodes)
        {
            var ind = BoardCellIndicator.Create(eater.GlobalPosition, eater.BoardStatePositionId, eater);
            _indicators.AddChild(ind);
            _boardCellIndicatorMapping.Add(eater.BoardStatePositionId, ind);
            cutscenes.Add(new(CreatePopNodeCutsceneAction(eater), cutsceneDelay));
        }

        EventManager.MovePerformed += HandleMove;
        EventManager.InvokeLevelReset();

        RandomUtils.Shuffle(cutscenes);
        CutsceneManager.Play(cutscenes);
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

    // Updated signature and using food object properties
    private void HandleMove(Eater eater, Food food, bool isHint)
    {
        if (isHint) // Use the new isHint parameter
        {
            HintManager.ActivateHint();
        }
        else
        {
            HintManager.HandleNonHintMove();
        }

        var unfinished = LevelManager.Level.GetUnfinishedFoodTypes();
        // Use food.FoodType and food.IsLast
        if (unfinished.Count == 1 && unfinished.FirstOrDefault() == food.FoodType && food.IsLast)
        {
            EventManager.InvokeLevelVictorious();
            ModalManager.OpenVictoryModal();
        }
    }

    private static Action CreatePopNodeCutsceneAction(Node2D node)
    {
        node.Scale = Vector2.Zero;
        return () => 
        {
            AudioManager.PlayAudio(AudioType.Undo, 1.5f);
            TweenUtils.Pop(node, 1);
        };
    }
}
