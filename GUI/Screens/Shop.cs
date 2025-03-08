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
        _eaterDisplay.BaseScale = 4;
    }

    public void UpdateEaterDisplay(EaterFace eaterFace)
    {
        _eaterDisplay.EaterFace = eaterFace;
        _eaterDisplay.EaterType = EnumUtils.GetRandomValueExcluding(new EaterType[1] { EaterType.Hidden });
        _eaterDisplay.Scale = new Vector2(2, 2);
        TweenUtils.Pop(_eaterDisplay, _eaterDisplay.BaseScale);
        _eaterDisplay.Setup();
    }
}
