public partial class RetryButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        ModalManager.CloseModal(overideUnclosable: true);
        HistoryManager.ResetHistory();
        LevelManager.ResetLevel();
        HintManager.CalculateSolutionPath();

        EventManager.InvokeLevelReset();
    }
}
