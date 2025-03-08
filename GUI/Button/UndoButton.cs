using Godot;

public partial class UndoButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => HistoryManager.MoveCount > 0 && HistoryManager.UndoCount > 0;

        SignalProvider.Instance.LevelReset += HandleLevelReset;
    }
    
    protected override void OnClick()
    {
        HistoryManager.UndoMove();
        AudioManager.PlayAudio(AudioType.Undo);
        HintManager.HandleUndo();

        SetCustomText($"UNDO [{HistoryManager.UndoCount}]");
    }

    public override void _Notification(int what)
    {
        base._Notification(what);

        if (what == NotificationPredelete)
        {
            SignalProvider.Instance.LevelReset -= HandleLevelReset;
        }
    }

    private void HandleLevelReset()
    {
        SetCustomText($"UNDO [{HistoryManager.UndoCount}]");
    }
}