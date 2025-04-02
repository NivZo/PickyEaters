public partial class DoubleRewardButton : CustomButton
{
    protected override void OnClick()
    {
        AcceptRewardButton.SetReward(CoinsManager.QuestReward * 2);
    }

}
