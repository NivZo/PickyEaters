public partial class CollectionButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        AudioManager.PlayAudio(AudioType.FoodConsumed);
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.Collection);
    }

}
