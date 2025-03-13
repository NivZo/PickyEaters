using Godot;

public partial class PlaySelectedLevelButton : CustomButton
{
    [Export] public int LevelId;

    public override void _EnterTree()
    {
        CustomText = LevelId.ToString();
        base._EnterTree();

        
        Scale = Vector2.Zero;
        TweenUtils.Pop(this, 1);

        var stars = SaveManager.ActiveSave.LevelStarsObtained[LevelId];

        GetNode<Sprite2D>("Stars/StarL").Visible = stars > 0;
        GetNode<Sprite2D>("Stars/StarM").Visible = stars > 1;
        GetNode<Sprite2D>("Stars/StarR").Visible = stars > 2;
    }

    protected override void OnClick()
    {
        LevelManager.CurrentLevelId = LevelId;
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.PlayScreen);
    }
}
