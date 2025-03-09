using Godot;

public partial class UndoButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => HistoryManager.MoveCount > 0;

        SignalProvider.Instance.LevelReset += HandleLevelReset;
    }
    
    protected override void OnClick()
    {
        if (HistoryManager.UndoCount > 0)
        {
            HistoryManager.UndoMove();
            AudioManager.PlayAudio(AudioType.Undo);
            HintManager.HandleUndo();

            SetCustomText($"UNDO [{HistoryManager.UndoCount}]");
        }
        else
        {
            ModalManager.OpenAreYouSureModal(() => {
                HistoryManager.ResetUndos();
                SetCustomText($"UNDO [{HistoryManager.UndoCount}]");
            },
            "OUT OF UNDOS!\nWATCH AN AD TO REFILL?");
        }
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