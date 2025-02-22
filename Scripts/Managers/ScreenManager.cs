using System;
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

        SignalProvider.Instance.FadeOutTranistionFinished += PerformTransition;
    }

    public static void LoadFirstScreen()
    {
        Main.ErrorDisplay = "LoadFirst1";
        _screenToLoad = ScreenType.MainMenu;
        Main.ErrorDisplay = "LoadFirst2";
        PerformTransition();
        Main.ErrorDisplay = "LoadFirst3";
    }
    
    public static void TransitionToScreen(ScreenType screenType)
    {
        _screenToLoad = screenType;
        _transitionScreen.FadeOut();
    }

    private static void PerformTransition()
    {
        Main.ErrorDisplay = "Perf1";
        foreach (var child in _screenLayer.GetChildren())
        {
            child.QueueFree();
        }
        Main.ErrorDisplay = "Perf2";
        
        CurrentScreen = _screenToLoad;
        Main.ErrorDisplay = $"MainMenu - {GD.Load($"res://GUI/Screens/MainMenu.tscn") == null} , PlayScreen - {GD.Load($"res://GUI/Screens/PlayScreen.tscn") == null} , Collection - {GD.Load($"res://GUI/Screens/Collection.tscn") == null}";
        _currentScreen = GetScreen(CurrentScreen);
        Main.ErrorDisplay = "Perf4";
        _screenLayer.AddChild(_currentScreen);
        Main.ErrorDisplay = "Perf5";
        _transitionScreen.FadeIn();
        Main.ErrorDisplay = "Perf6";

        SignalProvider.Emit(SignalProvider.SignalName.CoinsValueChanged);
        Main.ErrorDisplay = "Perf7";
    }

    private static Node GetScreen(ScreenType screenType)
        => GD.Load<PackedScene>($"res://GUI/Screens/{screenType}.tscn").Instantiate<Node>();

    public enum ScreenType
    {
        MainMenu,
        PlayScreen,
        Collection,
    }
}