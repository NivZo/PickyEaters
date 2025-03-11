using System;
using System.IO;
using Godot;

public class ScreenManager
{
    private static Node _currentScreen = null;
    private static CanvasLayer _screenLayer;
    private static Transition _transitionScreen;

    private static ScreenType _screenToLoad = ScreenType.MainMenu;
    public static ScreenType CurrentScreen = ScreenType.MainMenu;

    public static void Setup(CanvasLayer screenLayer, Transition transitionScreen)
    {
        _screenLayer = screenLayer;
        _transitionScreen = transitionScreen;

        EventManager.FadeOutTransitionFinished += PerformTransition;
    }

    public static void LoadFirstScreen()
    {
        _screenToLoad = ScreenType.MainMenu;
        PerformTransition();
    }
    
    public static void TransitionToScreen(ScreenType screenType)
    {
        _screenToLoad = screenType;
        _transitionScreen.FadeOut();
    }

    private static void PerformTransition()
    {
        foreach (var child in _screenLayer.GetChildren())
        {
            child.QueueFree();
        }
        CurrentScreen = _screenToLoad;
        _currentScreen = GetScreen(CurrentScreen);
        _screenLayer.AddChild(_currentScreen);
        _transitionScreen.FadeIn();
    }

    private static Node GetScreen(ScreenType screenType)
        => GD.Load<PackedScene>($"res://GUI/Screens/{screenType}.tscn").Instantiate<Node>();

    public enum ScreenType
    {
        MainMenu,
        PlayScreen,
        LevelSelection,
        Collection,
        Shop,
    }
}