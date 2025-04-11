using Godot;

public partial class Shop : Node
{
    public static Shop Instance { get; private set; }
    private EaterDisplay _eaterDisplay;
    private CpuParticles2D _purchaseParticles;

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;

        _eaterDisplay = GetNode<EaterDisplay>("GUILayer/MuncherUnlock/EaterDisplay");
        _eaterDisplay.BaseScale = 3;
        _purchaseParticles = GetNode<CpuParticles2D>("GUILayer/MuncherUnlock/PurchaseParticles");

        BackgroundManager.ChangeColor(NamedColor.Orange.GetColor(), lightenFactor: .7f);
    }
}
