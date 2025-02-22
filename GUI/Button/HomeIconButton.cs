public partial class HomeIconButton : CustomIconButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        AudioManager.PlayAudio(AudioType.Undo);
        ModalManager.CloseModal();
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.MainMenu);
    }

}
