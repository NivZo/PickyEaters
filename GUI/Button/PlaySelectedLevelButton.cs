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
    }
    
    protected override void OnClick()
    {
        LevelManager.CurrentLevelId = LevelId;
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.PlayScreen);
    }

}
