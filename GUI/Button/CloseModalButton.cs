public partial class CloseModalButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        base.OnClick();

        ModalManager.CloseModal(true);
    }

}
