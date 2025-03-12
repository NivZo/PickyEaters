public partial class ShopButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.Shop);
    }

}
