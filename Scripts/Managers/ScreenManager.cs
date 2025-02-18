using System;
using Godot;

public class ScreenManager
{
    public static ScreenManager Instance { get; } = new ScreenManager();

    private Node _currentScreen = null;
    private CanvasLayer _screenLayer;

    public ScreenType CurrentScreen = ScreenType.MainMenu;

    public void Setup(CanvasLayer screenLayer)
    {
        _screenLayer = screenLayer;
    }
    
    public void TransitionToScreen(ScreenType screenType)
    {
        foreach (var child in _screenLayer.GetChildren())
        {
            child.QueueFree();
        }
        
        CurrentScreen = screenType;
        _currentScreen = GetScreen(screenType);
        _screenLayer.AddChild(_currentScreen);
    }

    private Node GetScreen(ScreenType screenType)
        => GD.Load<PackedScene>($"res://GUI/Screens/{screenType}.tscn").Instantiate<Node>();

    public enum ScreenType
    {
        MainMenu,
        PlayScreen,
    }
}