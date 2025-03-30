using System;
using System.Collections.Generic;
using Godot;

public static class TweenUtils
{
    private static Dictionary<string, (Tween Tween, Variant Value)> _tweens = new();

    public static Tween Pop(Node node, float scale, float duration = 1f, Tween.TransitionType transitionType = Tween.TransitionType.Elastic)
        => AddPropertyTween(node, "scale", new Vector2(scale, scale), duration, transitionType);

    public static Tween Travel(Node node, Vector2 to, float duration = 0.5f, Tween.TransitionType transitionType = Tween.TransitionType.Quint)
        => AddPropertyTween(node, "global_position", to, duration, transitionType);

    public static void BoldOutline(Node node, int min, int max, float duration = 0.5f)
    {
        AddPropertyTween(node, "material:shader_parameter/minLineWidth", min, duration, Tween.TransitionType.Expo);
        AddPropertyTween(node, "material:shader_parameter/maxLineWidth", max, duration, Tween.TransitionType.Expo);
    }

    public static Tween MethodTween(Node node, Action<Variant> action, Variant from, Variant to, float duration, Tween.TransitionType transitionType = Tween.TransitionType.Cubic)
    {
        var tween = AddCachedTween(node, action.Method.Name, to, transitionType);
        if (tween != null)
        {
            tween.TweenMethod(Callable.From(action), from, to, duration);
        }

        return tween;
    }

    private static Tween AddPropertyTween(Node node, string property, Variant value, float duration, Tween.TransitionType transitionType)
    {
        var tween = AddCachedTween(node, property, value, transitionType);
        if (tween != null)
        {
            tween.TweenProperty(node, property, value, duration);
        }

        return tween;
    }

    private static Tween AddCachedTween(Node node, string property, Variant value, Tween.TransitionType transitionType)
    {
        if (node != null && !node.IsQueuedForDeletion())
        {
            var tweenKey = $"{node.GetInstanceId()}-{property}";
            var exists = _tweens.TryGetValue(tweenKey, out var tweenTuple) && tweenTuple.Tween.IsRunning();

            if (exists && !tweenTuple.Value.Equals(value))
            {
                tweenTuple.Tween.Kill();
            }

            var tween = node?
                .CreateTween()
                .SetEase(Tween.EaseType.Out).SetTrans(transitionType)
                .SetParallel(true);

            if (tween != null)
            {
                _tweens.Remove(tweenKey);
                _tweens.Add(tweenKey, (tween, value));
            }

            return tween;
        }

        return null;
    }
}