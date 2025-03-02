using System.Linq;
using Godot;

public partial class HintButton : CustomButton
{
    private const int HINTS_PER_CLICK = 5;
    private int _currentClickHintsLeft = HINTS_PER_CLICK;
    private Timer _currentClickHintsTimer = new();

    public override void _Ready()
    {
        base._Ready();

        IsEnabledFunc = HintManager.IsHintAvailable;
        _currentClickHintsTimer.WaitTime = 0.2f;
        _currentClickHintsTimer.OneShot = false;
        _currentClickHintsTimer.Timeout += ExecuteHintInternal;
        AddChild(_currentClickHintsTimer);
    }
    
    protected override void OnClick()
    {
        _currentClickHintsTimer.Start();
        ActionManager.StartBackgroundAction();
    }

    private void ExecuteHintInternal()
    {
        if (_currentClickHintsLeft > 0 && !LevelManager.Instance.IsVictory())
        {
            var firstMove = HintManager.GetHint();
            if (firstMove != null)
            {
                var eater = LevelManager.Instance.Level.GetEaters().FirstOrDefault(eater => eater.BoardStatePositionId == firstMove.From);
                var food = LevelManager.Instance.Level.GetFood().FirstOrDefault(food => food.BoardStatePositionId == firstMove.To);
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
            _currentClickHintsLeft = HINTS_PER_CLICK;
            ActionManager.FinishBackgroundAction();
        }
    }
}
