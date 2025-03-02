public partial class PlayButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        LevelManager.Instance.CurrentLevelId = SaveManager.ActiveSave.LevelReached;
        AudioManager.PlayAudio(AudioType.FoodConsumed);
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.PlayScreen);
    }

}
