using Godot;

public partial class Shop : Node
{
    public static Shop Instance { get; private set; }
    private EaterDisplay _eaterDisplay;

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;

        _eaterDisplay = GetNode<EaterDisplay>("GUILayer/MuncherUnlock/EaterDisplay");
        _eaterDisplay.BaseScale = 3;
        
        BackgroundManager.ChangeColor(NamedColor.Orange.GetColor(), lightenFactor: .7f);
    }

    public void UpdateEaterDisplay(EaterFace eaterFace)
    {
        CutsceneManager.Play(new() {
            new(() => {
                TweenUtils.Pop(_eaterDisplay, 2f, 3f, transitionType: Tween.TransitionType.Spring);
            }, 0),
            new(() => {        
                _eaterDisplay.EaterFace = eaterFace;
                _eaterDisplay.EaterType = EnumUtils.GetRandomValueExcluding(new EaterType[1] { EaterType.Hidden });
                _eaterDisplay.Setup();
            }, 2),
            new(() => {
                TweenUtils.Pop(_eaterDisplay, 3.5f, 1f);
            }, 0),
        });
        // _eaterDisplay.EaterFace = eaterFace;
        // _eaterDisplay.EaterType = EnumUtils.GetRandomValueExcluding(new EaterType[1] { EaterType.Hidden });
        // _eaterDisplay.Scale = new Vector2(2, 2);
        // TweenUtils.Pop(_eaterDisplay, _eaterDisplay.BaseScale);
        // _eaterDisplay.Setup();
    }
}
