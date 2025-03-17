using System;
using System.Collections.Generic;
using Godot;

public static class CutsceneManager
{
    private static Timer _timer;
    private static Action _nextAction;
    private static Queue<CutsceneAction> _queue = new();

    // Take Action after Delay seconds
    public record CutsceneAction(Action Action, float Delay);

    public static void Setup(Main main)
    {
        _timer = new Timer() { Autostart = false, OneShot = true };
        _timer.Timeout += HandleTimeout;
        main.AddChild(_timer);
    }

    public static void Play(List<CutsceneAction> cutsceneActions)
    {
        var isEmpty = _queue.Count == 0;
        _queue.Enqueue(new(ActionManager.StartBackgroundAction, 0f));
        cutsceneActions.ForEach(_queue.Enqueue);
        _queue.Enqueue(new(ActionManager.FinishBackgroundAction, 0f));

        if (isEmpty)
        {
            PlayInternal();
        }
    }

    private static void PlayInternal()
    {
        var next = _queue.Dequeue();
        _nextAction = next.Action;
        if (next.Delay == 0)
        {
            HandleTimeout();
        }
        else
        {
            _timer.Start(next.Delay);
        }
    }

    private static void HandleTimeout()
    {
        _nextAction();
        if (_queue.Count != 0)
        {
            PlayInternal();
        }
        else
        {
            _timer.Stop();
        }
    }
}