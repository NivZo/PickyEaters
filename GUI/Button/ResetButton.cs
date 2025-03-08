public partial class ResetButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => HistoryManager.MoveCount > 0;
    }
    
    protected override void OnClick()
    {
        ModalManager.OpenAreYouSureModal(() => {
            HistoryManager.ResetHistory();
            LevelManager.ResetLevel();
            HintManager.CalculateSolutionPath();

            SignalProvider.Emit(SignalProvider.SignalName.LevelReset);
        });

        AudioManager.PlayAudio(AudioType.Undo);
    }
}
