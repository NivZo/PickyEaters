public partial class PlayButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        AudioManager.Instance.PlayAudio(AudioType.FoodConsumed);
        ScreenManager.Instance.TransitionToScreen(ScreenManager.ScreenType.PlayScreen);
    }

}
