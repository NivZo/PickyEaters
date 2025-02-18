public partial class UndoButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => HistoryManager.Instance.MoveCount > 0;
    }
    
    protected override void OnClick()
    {
        HistoryManager.Instance.UndoMove();
        AudioManager.Instance.PlayAudio(AudioType.Undo);
    }

}
