public partial class HomeButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        ModalManager.CloseModal(overideUnclosable: true);
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.MainMenu);
    }

}
