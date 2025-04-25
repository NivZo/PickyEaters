using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Level : Node
{
    private const int _maxIndependentFoodPops = 12;
    public Node Food;
    public Node Eaters;
    private Node _indicators;
    private Dictionary<Vector2I, BoardCellIndicator> _boardCellIndicatorMapping = new();
    private HashSet<EaterFace> _facesInLevel = new();
    
    public override void _Ready()
    {
        base._Ready();

        Food = GetNode<Node>("Food");
        Eaters = GetNode<Node>("Eaters");

        _indicators = new Node();
        AddChild(_indicators);

        var cutscenes = new List<CutsceneManager.CutsceneAction>();
        var eaterNodes = GetEaters();
        var foodNodes = GetFood();
        RandomUtils.Shuffle(foodNodes);

        int foodBatchSize = 1;
        if (foodNodes.Count > _maxIndependentFoodPops)
        {
            foodBatchSize = (int)Math.Ceiling((float)foodNodes.Count / _maxIndependentFoodPops);
        }

        var numberOfAnimations = Math.Min(foodNodes.Count / foodBatchSize, _maxIndependentFoodPops) + eaterNodes.Count;
        var cutsceneDelay = 1f / numberOfAnimations;

        for (int i = 0; i < foodNodes.Count; i += foodBatchSize)
        {
            var batch = foodNodes.Skip(i).Take(foodBatchSize).ToArray();
            var batchAction = CreatePopNodeCutsceneAction(batch);

            foreach (var food in batch)
            {
                var ind = BoardCellIndicator.Create(food.GlobalPosition, food.BoardStatePositionId);
                _indicators.AddChild(ind);
                _boardCellIndicatorMapping.Add(food.BoardStatePositionId, ind);
            }

            cutscenes.Add(new(batchAction, cutsceneDelay));
        }
        RandomUtils.Shuffle(cutscenes);

        foreach (var eater in eaterNodes)
        {
            var ind = BoardCellIndicator.Create(eater.GlobalPosition, eater.BoardStatePositionId, eater);
            _indicators.AddChild(ind);
            _boardCellIndicatorMapping.Add(eater.BoardStatePositionId, ind);
            cutscenes.Insert(0, new(CreatePopNodeCutsceneAction(eater), cutsceneDelay));
        }

        EventManager.MovePerformed += HandleMove;
        EventManager.InvokeLevelReset();

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

    public EaterFace GetAvailableEaterFace()
    {
        var availableFaces = SaveManager.ActiveSave.UnlockedFaces
            .Where(face => face != EaterFace.Hidden && !_facesInLevel.Contains(face))
            .ToList();
        if (availableFaces.Count == 0)
        {
            return EaterFace.SmileBasic;
        }

        var randomFace = EnumUtils.GetRandomValueOutOf(availableFaces);
        _facesInLevel.Add(randomFace);
        return randomFace;
    }
    
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

    private void HandleMove(Eater eater, Food food, bool isHint)
    {
        if (isHint)
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

    private static Action CreatePopNodeCutsceneAction(params Node2D[] nodes)
    {
        foreach (var node in nodes)
        {
            node.Scale = Vector2.Zero;
        }
        
        return () =>
        {
            AudioManager.PlaySoundEffect(AudioType.Pop);
            foreach (var node in nodes)
            {
                TweenUtils.Pop(node, 1);
            }
        };
    }
}
