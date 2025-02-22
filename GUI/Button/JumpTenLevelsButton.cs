using Godot;

public partial class JumpTenLevelsButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => LevelManager.Instance.CurrentLevelId < LevelManager.Instance.MaxLevel &&
            (LevelManager.Instance.CurrentLevelId < SaveManager.ActiveSave.LevelReached);
    }
    
    protected override void OnClick()
    {
        AudioManager.PlayAudio(AudioType.Undo);
        
        LevelManager.Instance.NextTenLevels();
        SaveManager.SaveGame();

        if (ModalManager.CurrentOpenModal !=ModalManager.ModalType.None)
        {
            ModalManager.CloseModal();
        }
    }

}
