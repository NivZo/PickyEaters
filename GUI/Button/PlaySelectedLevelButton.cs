using Godot;

public partial class PlaySelectedLevelButton : CustomButton
{
    [Export] public int LevelId;

    private static readonly string[] _colors = new[]
    {
        "7390ea", // EASY
        "fec851", // MEDIUM
        "d85058", // HARD
        "ff7dbb", // EXPERT
        "7b51cf", // GENIUS
        "944d12", // SUPER
        "2c2c2c", // MASTER
    };
    
    public override void _EnterTree()
    {
        CustomText = LevelId.ToString();
        base._EnterTree();

        
        Scale = Vector2.Zero;
        TweenUtils.Pop(this, 1, duration: 0.5f, transitionType: Tween.TransitionType.Quint);

        var stars = SaveManager.ActiveSave.LevelStarsObtained[LevelId];

        GetNode<Sprite2D>("Stars/StarL").Visible = stars > 0;
        GetNode<Sprite2D>("Stars/StarM").Visible = stars > 1;
        GetNode<Sprite2D>("Stars/StarR").Visible = stars > 2;

        Color = new Color(_colors[(LevelId-1) / 75]);
    }

    protected override void OnClick()
    {
        LevelManager.CurrentLevelId = LevelId;
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.PlayScreen);
    }
}
