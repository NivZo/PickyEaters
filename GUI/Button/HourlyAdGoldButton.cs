using System;
using Godot;

public partial class HourlyAdGoldButton : CustomButton
{
    public const int HOURLY_REWARD_AMOUNT = 100;

    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => ShopStacksManager.IsHourlyRewardAvailable();
        WaveString = IsEnabled();
        EventManager.AdRewardGranted += OnRewardGranted;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventManager.AdRewardGranted -= OnRewardGranted;
    }

    protected override void OnClick()
    {
        EventManager.InvokeAdRewardRequested("hourly_gold");
    }

    private void OnRewardGranted(string rewardType)
    {
        if (rewardType == "hourly_gold")
        {
            EventManager.InvokeHourlyGoldButtonClicked();
            ShopStacksManager.ConsumeHourlyReward();
            CoinsManager.AddCoins(HOURLY_REWARD_AMOUNT);
            WaveString = false;
        }
    }
}
