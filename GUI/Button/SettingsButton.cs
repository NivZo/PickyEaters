public partial class SettingsButton : CustomIconButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        AudioManager.PlayAudio(AudioType.Undo);
        ModalManager.OpenSettingsModal();
    }

}
