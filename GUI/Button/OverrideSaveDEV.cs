public partial class OverrideSaveDEV : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        SaveManager.OverrideDevSave();
        ModalManager.CloseModal();
    }

}