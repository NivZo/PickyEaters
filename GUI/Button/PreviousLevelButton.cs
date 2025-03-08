using System;

public partial class PreviousLevelButton : CustomButton
{
    public override void _Ready()
    {
        base._Ready();
        IsEnabledFunc = () => LevelManager.CurrentLevelId > 1;
    }
    
    protected override void OnClick()
    {
        LevelManager.PreviousLevel();
        AudioManager.PlayAudio(AudioType.Undo);
    }

}
