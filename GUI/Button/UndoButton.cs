using Godot;

public partial class UndoButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => HistoryManager.MoveCount > 0;

        EventManager.LevelReset += HandleLevelReset;
    }
    
    protected override void OnClick()
    {
        if (HistoryManager.UndoCount > 0)
        {
            HistoryManager.UndoMove();
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

    private void HandleLevelReset()
    {
        SetCustomText($"UNDO [{HistoryManager.UndoCount}]");
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        EventManager.LevelReset -= HandleLevelReset;
    }
}