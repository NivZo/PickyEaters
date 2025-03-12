public partial class NextLevelButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => LevelManager.CurrentLevelId < LevelManager.MaxLevel &&
            (LevelManager.CurrentLevelId < SaveManager.ActiveSave.LevelReached);
    }
    
    protected override void OnClick()
    {
        LevelManager.NextLevel();

        if (ModalManager.CurrentOpenModal !=ModalManager.ModalType.None)
        {
            ModalManager.CloseModal(overideUnclosable: true);
        }
    }

}
