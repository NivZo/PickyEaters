public partial class HomeButton : CustomIconButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        AudioManager.Instance.PlayAudio(AudioType.Undo);
        ModalManager.Instance.CloseModal();
        ScreenManager.Instance.TransitionToScreen(ScreenManager.ScreenType.MainMenu);
    }

}
