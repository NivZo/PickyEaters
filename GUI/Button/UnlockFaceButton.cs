using System;
using System.Linq;

public partial class UnlockFaceButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => SaveManager.ActiveSave.UnlockedFaces.Count < (Enum.GetValues<EaterFace>().Length-1)
            && SaveManager.ActiveSave.Coins >= 1000;
    }
    
    protected override void OnClick()
    {
        AudioManager.PlayAudio(AudioType.FoodConsumed);
        var newFace = EnumUtils.GetRandomValueExcluding(SaveManager.ActiveSave.UnlockedFaces.Concat(new EaterFace[1] { EaterFace.Hidden }).ToList());
        SaveManager.ActiveSave.UnlockedFaces.Add(newFace);
        SaveManager.ActiveSave.Coins -= 1000;
        SaveManager.SaveGame();

        SignalProvider.Emit(SignalProvider.SignalName.ActiveSaveChanged);

        Shop.Instance.UpdateEaterDisplay(newFace);
    }

}
