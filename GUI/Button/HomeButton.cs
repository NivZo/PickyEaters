public partial class HomeButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        AudioManager.PlayAudio(AudioType.Undo);
        ModalManager.CloseModal(overideUnclosable: true);
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.MainMenu);
    }

}
