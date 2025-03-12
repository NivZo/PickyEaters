public partial class PlayButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        LevelManager.CurrentLevelId = SaveManager.ActiveSave.LevelReached;
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.PlayScreen);
    }

}
