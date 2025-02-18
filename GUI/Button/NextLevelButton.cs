public partial class NextLevelButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => LevelManager.Instance.CurrentLevelId < LevelManager.Instance.MaxLevel &&
            (LevelManager.Instance.CurrentLevelId < SaveManager.ActiveSave.LevelReached);
    }
    
    protected override void OnClick()
    {
        AudioManager.Instance.PlayAudio(AudioType.Undo);
        
        LevelManager.Instance.NextLevel();

        if (ModalManager.Instance.CurrentOpenModal !=ModalManager.ModalType.None)
        {
            ModalManager.Instance.CloseModal(overideUnclosable: true);
        }
    }

}
