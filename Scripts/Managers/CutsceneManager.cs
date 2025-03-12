using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public static class CutsceneManager
{
    private static Timer _timer;
    private static Action _nextAction;

    public record CutsceneAction(Action Action, float Delay);

    public static void Setup(Main main)
    {
        _timer = new Timer() { Autostart = false, OneShot = true };
        _timer.Timeout += HandleTimeout;
        main.AddChild(_timer);
    }

    private static Queue<CutsceneAction> _queue = new();

    public static void Play(List<CutsceneAction> cutsceneActions)
    {
        cutsceneActions.ForEach(_queue.Enqueue);
        _queue.Enqueue(new(ActionManager.FinishBackgroundAction, 0.05f));
        
        ActionManager.StartBackgroundAction();
        var first = _queue.Dequeue();
        _nextAction = first.Action;
        _timer.Start(first.Delay);
    }

    private static void HandleTimeout()
    {
        _nextAction();
        if (_queue.Count == 0)
        {
            _timer.Stop();
        }
        else
        {
            var next = _queue.Dequeue();
            _nextAction = next.Action;
            _timer.Start(next.Delay);
        }
    }
}