using System.Linq;
using Godot;

public partial class HintButton : CustomIconButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        var firstMove = HintManager.GetHint();
        if (firstMove != null)
        {
            firstMove.Eater.PerformMove(firstMove.FoodAtTarget);
            HintManager.ActivateHint();
        }
    }

}
