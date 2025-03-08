public partial class HomeIconButton : CustomIconButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        AudioManager.PlayAudio(AudioType.Undo);
        if (ScreenManager.CurrentScreen == ScreenManager.ScreenType.PlayScreen)
        {
            ModalManager.OpenAreYouSureModal(() => {
                ModalManager.CloseModal(overideUnclosable: true);
                ScreenManager.TransitionToScreen(ScreenManager.ScreenType.MainMenu);
            });
        }
        else
        {
            ModalManager.CloseModal(overideUnclosable: true);
            ScreenManager.TransitionToScreen(ScreenManager.ScreenType.MainMenu);
        }
    }

}
