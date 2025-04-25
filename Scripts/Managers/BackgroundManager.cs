using System;
using Godot;

public class BackgroundManager
{
    private static ColorRect _background;
    private static ColorRect _transitionBackground;

    public static void Setup()
    {
        _background = Main.Instance.GetNode<ColorRect>("BackgroundLayer/BackgroundEffectTiled");
        _transitionBackground = Main.Instance.GetNode<ColorRect>("Shaders/Transition/BackgroundEffectTiled");
    }

    public static void ChangeColor(Color target, float duration = 1.5f, float lightenFactor = 0.35f)
    {
        target = target.Lightened(lightenFactor);
        TweenUtils.Color(_background, target, duration);
        TweenUtils.Color(_transitionBackground, target, duration);
    }
}