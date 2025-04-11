using System;
using Godot;

public partial class DailyFreeGoldButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => SaveManager.ActiveSave.LastDailyFreeRewardClaimedOLE < DateTime.Now.Date.ToOADate();
    }
    
    protected override void OnClick()
    {
        SaveManager.ActiveSave.LastDailyFreeRewardClaimedOLE = DateTime.Now.Date.ToOADate();
        CoinsManager.AddCoins(250);
        WaveString = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!IsEnabledFunc())
        {
            SetCustomText(TimeLeftToClaim());
        }
    }

    private string TimeLeftToClaim()
    {
        var nextClaimTime = DateTime.FromOADate(SaveManager.ActiveSave.LastDailyFreeRewardClaimedOLE).AddDays(1);
        var timeLeft = nextClaimTime > DateTime.Now ? nextClaimTime - DateTime.Now : TimeSpan.Zero;
        return $"{(timeLeft.Hours < 0 ? 0 : timeLeft.Hours):D2}:{(timeLeft.Minutes < 0 ? 0 : timeLeft.Minutes):D2}:{(timeLeft.Seconds < 0 ? 0 : timeLeft.Seconds):D2}";
    }
}
