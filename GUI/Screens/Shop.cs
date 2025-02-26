using Godot;

public partial class Shop : Node
{
    public static Shop Instance { get; private set; }
    private EaterDisplay _eaterDisplay;

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;

        _eaterDisplay = GetNode<EaterDisplay>("EaterDisplay");
    }

    public void UpdateEaterDisplay(EaterFace eaterFace)
    {
        _eaterDisplay.EaterFace = eaterFace;
        _eaterDisplay.EaterType = EnumUtils.GetRandomValueExcluding(new EaterType[1] { EaterType.Hidden });
        _eaterDisplay.Scale = new(2, 2);
        TweenUtils.Pop(_eaterDisplay, 4);
        _eaterDisplay.Setup();
    }
}
