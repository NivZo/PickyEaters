using System;

public partial class PreviousLevelButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => LevelManager.Instance.CurrentLevelId > 1;
    }
    
    protected override void OnClick()
    {
        LevelManager.Instance.PreviousLevel();
        AudioManager.Instance.PlayAudio(AudioType.Undo);
    }

}
