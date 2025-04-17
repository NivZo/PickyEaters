public partial class DoubleRewardButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        EventManager.AdRewardGranted += OnRewardGranted;
    }

    protected override void OnClick()
    {
        AdmobProvider.Instance.ShowRewardedAd("double_gold", false);
        SetCustomText("LOADING");
    }

    private void OnRewardGranted(string rewardType)
    {
        if (rewardType == "double_gold")
        {
            SetCustomText("DONE!");
            IsEnabledFunc = () => false;
            AcceptRewardButton.SetReward(CoinsManager.QuestReward * 2);
        }
    }
}
