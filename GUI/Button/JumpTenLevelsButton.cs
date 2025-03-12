using Godot;

public partial class JumpTenLevelsButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => LevelManager.CurrentLevelId < LevelManager.MaxLevel &&
            (LevelManager.CurrentLevelId < SaveManager.ActiveSave.LevelReached);
    }
    
    protected override void OnClick()
    {
        LevelManager.NextTenLevels();
        SaveManager.CommitActiveSave();

        if (ModalManager.CurrentOpenModal !=ModalManager.ModalType.None)
        {
            ModalManager.CloseModal(overideUnclosable: true);
        }
    }

}
