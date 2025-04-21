using System;
using Godot;

public partial class DailyFreeGoldButton : CustomButton
{
    private const int DAILY_REWARD_AMOUNT = 250;

    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => ShopStacksManager.IsDailyRewardAvailable();
    }

    protected override void OnClick()
    {
        EventManager.InvokeDailyGoldButtonClicked();
        ShopStacksManager.ConsumeDailyReward();
        CoinsManager.AddCoins(DAILY_REWARD_AMOUNT);
        WaveString = false;
    }
}
