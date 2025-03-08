using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Level : Node
{
    public Node Food;
    public Node Eaters;
    
    public override void _Ready()
    {
        base._Ready();

        Food = GetNode<Node>("Food");
        Eaters = GetNode<Node>("Eaters");

        SignalProvider.Instance.MovePerformed += HandleMove;
        HintManager.CalculateSolutionPath();
    }

    public override void _Notification(int what)
    {
        base._Notification(what);

        if (what == NotificationPredelete)
        {
            SignalProvider.Instance.MovePerformed -= HandleMove;
        }
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
