using System;
using Godot;

public class ActionManager
{
    public static ActionManager Instance { get; } = new ActionManager();

    private bool _isActionAvailable = true;
    private Action _onFinish;
    private Node _actor;
    
    public Node Actor => _actor;
    public bool IsActionAvailable() => _isActionAvailable;

    public void StartAction(Node actor, Action callback)
    {
        _onFinish = callback;
        _isActionAvailable = false;
        _actor = actor;
    }
    
    public void FinishAction()
    {
        if (_onFinish is not null)
        {
            _onFinish();
            _onFinish = null;
            _actor = null;
        }

        _isActionAvailable = true;
    }
}