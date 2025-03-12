public partial class LevelSelectionButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.LevelSelection);
    }

}
