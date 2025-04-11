using System;
using System.Linq;

public partial class UnlockFaceButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => SaveManager.ActiveSave.UnlockedFaces.Count < (Enum.GetValues<EaterFace>().Length-1)
            && SaveManager.ActiveSave.Coins >= 1000
            && ModalManager.CurrentOpenModal == ModalManager.ModalType.None;
    }
    
    protected override void OnClick()
    {
        ModalManager.OpenMuncherUnlockModal();
    }

}
