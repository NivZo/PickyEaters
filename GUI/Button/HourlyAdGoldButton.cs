using System;

public partial class HourlyAdGoldButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => 
        {
            DateTime lastClaimWindowStart;
            try
            {
                lastClaimWindowStart = GetStartOfWindow(DateTime.FromOADate(SaveManager.ActiveSave.LastHourlyAdRewardClaimedOLE));
            }
            catch (ArgumentException)
            {
                lastClaimWindowStart = GetStartOfWindow(DateTime.MinValue);   
            }
            return GetStartOfWindow(DateTime.Now) > lastClaimWindowStart;
        };
    }
    
    protected override void OnClick()
    {
        SaveManager.ActiveSave.LastHourlyAdRewardClaimedOLE = DateTime.Now.ToOADate();
        CoinsManager.AddCoins(100);
        WaveString = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!IsEnabled())
        {
            SetCustomText(TimeLeftToClaim());
        }
    }

    private string TimeLeftToClaim()
    {
        DateTime nextWindowStart = GetStartOfWindow(DateTime.Now).AddHours(4);
        var timeLeft = nextWindowStart - DateTime.Now;
        return $"{(timeLeft.Hours < 0 ? 0 : timeLeft.Hours):D2}:{(timeLeft.Minutes < 0 ? 0 : timeLeft.Minutes):D2}:{(timeLeft.Seconds < 0 ? 0 : timeLeft.Seconds):D2}";
    }

    private DateTime GetStartOfWindow(DateTime dt)
    {
        // Calculate the hour that starts the window (0, 4, 8, 12, 16, 20)
        int windowStartHour = dt.Hour / 4 * 4;
        return new DateTime(dt.Year, dt.Month, dt.Day, windowStartHour, 0, 0, dt.Kind);
    }
}
