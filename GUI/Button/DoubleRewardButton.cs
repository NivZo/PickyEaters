public partial class DoubleRewardButton : CustomButton
{
    protected override void OnClick()
    {
        AcceptRewardButton.SetReward(300);
    }

}
