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

        // SignalProvider.Instance.MovePerformed += HandleMove;
        HintManager.CalculateSolutionPath();
    }

    public List<Eater> GetEaters() => Eaters
        .GetChildren()
        .Where(child => child is Eater)
        .Select(eater => eater as Eater).ToList();

    
    public List<Food> GetFood() => Food
        .GetChildren()
        .Where(child => child is Food)
        .Select(eater => eater as Food).ToList();
    
    private void HandleMove(Vector2I EaterPosId, Vector2I FoodPosId)
    {
        HintManager.CalculateSolutionPath();
        // var hint = HintManager.GetHint();
        // if (hint != null
        //     && (hint.Eater.BoardStatePositionId != EaterPosId || hint.FoodAtTarget.BoardStatePositionId != FoodPosId))
        // {
        //     HintManager.CalculateSolutionPath();
        // }
    }
}
