public partial class ResetButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => HistoryManager.Instance.MoveCount > 0;
    }
    
    protected override void OnClick()
    {
        HistoryManager.Instance.ResetHistory();
        LevelManager.Instance.ResetLevel();
        AudioManager.PlayAudio(AudioType.Undo);
    }

}
