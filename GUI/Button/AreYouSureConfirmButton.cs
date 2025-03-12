using System;

public partial class AreYouSureConfirmButton : CustomButton
{
    public Action OnConfirm;

    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        base.OnClick();
        
        OnConfirm();
    }

}
