using System;
using Godot;

public static class ActionManager
{
    private static bool _isPlayerActionAvailable = true;
    private static bool _isBackgroundActionAvailable = true;
    private static Action _onFinish;
    private static Node _actor;
    
    public static Node Actor => _actor;
    public static bool IsPlayerActionAvailable() => _isPlayerActionAvailable;
    public static bool IsBackgroundActionAvailable() => _isBackgroundActionAvailable;
    public static bool IsActionAvailable() => _isPlayerActionAvailable && _isBackgroundActionAvailable;

    public static void StartPlayerAction(Node actor, Action callback)
    {
        _onFinish = callback;
        _isPlayerActionAvailable = false;
        _actor = actor;
    }
    
    public static void FinishPlayerAction()
    {
        if (_onFinish is not null)
        {
            _onFinish();
            _onFinish = null;
            _actor = null;
        }

        _isPlayerActionAvailable = true;
    }

    public static void StartBackgroundAction() => _isBackgroundActionAvailable = false;
    public static void FinishBackgroundAction() => _isBackgroundActionAvailable = true;
}