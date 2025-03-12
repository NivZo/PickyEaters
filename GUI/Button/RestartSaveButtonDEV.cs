public partial class RestartSaveButtonDEV : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
    }
    
    protected override void OnClick()
    {
        SaveManager.EraseSave();
        ModalManager.CloseModal();
    }

}