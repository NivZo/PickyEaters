using Godot;

public partial class PlaySelectedLevelButton : CustomButton
{
    [Export] public int LevelId;

    public override void _EnterTree()
    {
        CustomText = LevelId.ToString();
        base._EnterTree();
    }
    
    protected override void OnClick()
    {
        LevelManager.Instance.CurrentLevelId = LevelId;
        AudioManager.PlayAudio(AudioType.FoodConsumed);
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.PlayScreen);
    }

}
