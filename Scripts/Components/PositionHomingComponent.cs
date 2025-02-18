using System;
using System.Collections.Generic;
using Godot;

public class TargetPositionComponent
{
    public Vector2 TargetPosition =>  _pin + _currentNudge;
    public Vector2 NudgelessTargetPosition => _pin;

    private Node2D _node;
    private Vector2 _pin = Vector2.Zero;
    private Vector2 _currentNudge = Vector2.Zero;

    public TargetPositionComponent(Node2D node)
    {
        _node = node;
    }

    public void SetPinPosition(Vector2? pin = null)
    {
        _pin = pin ?? _node.GlobalPosition;
    }

    public void Nudge(Vector2 direction)
    {
        if (direction != _currentNudge)
        {
            _currentNudge = direction;
        }
    }

    public void ResetNudge()
    {
        _currentNudge = Vector2.Zero;
    }
}
