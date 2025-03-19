using System;
using Godot;

public partial class PlaySelectedLevelButton : CustomButton
{
    [Export] public int LevelId;

    private static readonly Color[] _colors = new[]
    {
        TierColor.Easy.GetColor(),
        TierColor.Medium.GetColor(),
        TierColor.Hard.GetColor(),
        TierColor.Expert.GetColor(),
        TierColor.Genius.GetColor(),
        TierColor.Super.GetColor(),
        TierColor.Master.GetColor(),
    };
    
    public override void _EnterTree()
    {
        CustomText = LevelId.ToString();
        base._EnterTree();

        
        Scale = Vector2.Zero;
        TweenUtils.Pop(this, 1, duration: 0.5f, transitionType: Tween.TransitionType.Quint);

        var stars = SaveManager.ActiveSave.LevelStarsObtained[LevelId];

        GetNode<Sprite2D>("Stars/StarL").Visible = stars > 0;
        GetNode<Sprite2D>("Stars/StarM").Visible = stars > 1;
        GetNode<Sprite2D>("Stars/StarR").Visible = stars > 2;

        Color = new Color(_colors[Math.Max(0, (LevelId-1) / 75)]);
        IsEnabledFunc = () => LevelId <= SaveManager.ActiveSave.LevelReached;
        if (LevelId <= SaveManager.ActiveSave.LevelReached)
        {
            CustomIcon = null;
        }
    }

    protected override void OnClick()
    {
        LevelManager.CurrentLevelId = LevelId;
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.PlayScreen);
    }
}
