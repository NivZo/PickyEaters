using System.Linq;
using Godot;

public partial class HintButton : CustomButton
{
    private int _currentClickHintsLeft = 0;
    private Timer _currentClickHintsTimer = new();

    public override void _Ready()
    {
        base._Ready();

        IsEnabledFunc = HintManager.IsHintAvailable;

        _currentClickHintsTimer.WaitTime = 0.2f;
        _currentClickHintsTimer.OneShot = false;
        _currentClickHintsTimer.Timeout += ExecuteHintInternal;
        AddChild(_currentClickHintsTimer);
        SetCustomText($"HINT [{HintManager.HintsLeft}]");

        SignalProvider.Instance.LevelReset += HandleLevelReset;
    }
    
    protected override void OnClick()
    {
        if (!HintManager.IsOutOfHints())
        {
            UseHint();
        }
        else
        {
            ModalManager.OpenAreYouSureModal(() => {
                HintManager.ResetHintUsed();
                UseHint();
            },
            "OUT OF HINTS!\nWATCH AN AD TO REFILL?");
        }
    }

    private void UseHint()
    {
        _currentClickHintsTimer.Start();
        ActionManager.StartBackgroundAction();
        HintManager.HintUsed();
        SetCustomText($"HINT [{HintManager.HintsLeft}]");
    }

    private void ExecuteHintInternal()
    {
        if (_currentClickHintsLeft > 0 && !LevelManager.IsVictory())
        {
            var firstMove = HintManager.GetHint();
            if (firstMove != null)
            {
                var eater = LevelManager.Level.GetEaters().FirstOrDefault(eater => eater.BoardStatePositionId == firstMove.From);
                var food = LevelManager.Level.GetFood().FirstOrDefault(food => food.BoardStatePositionId == firstMove.To);
                if (eater != null & food != null)
                {
                    eater.PerformMove(food, true);
                }
                _currentClickHintsLeft--;
            }
            else
            {
                _currentClickHintsLeft = 0;
            }
        }
        else
        {
            _currentClickHintsTimer.Stop();
            _currentClickHintsLeft = HintManager.HintsPerClick;
            ActionManager.FinishBackgroundAction();
        }
    }

    private void HandleLevelReset()
    {
        HintManager.CalculateSolutionPath();
        _currentClickHintsLeft = HintManager.HintsPerClick;
        SetCustomText($"HINT [{HintManager.HintsLeft}]");
    }
}
