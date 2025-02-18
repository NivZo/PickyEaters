using System;
using System.Collections.Generic;
using Godot;

public class HoverComponent
{
    public bool IsHovered { get => _isHovered; }

    private bool _isHovered = false;
    private CollisionObject2D _collider;
    private List<Action> _onHoverStartAction = new();
    private List<Action> _onHoverEndAction = new();

    public HoverComponent(CollisionObject2D collider)
    {
        _collider = collider;
        _collider.MouseEntered += HandleMouseEnter;
        _collider.MouseExited += HandleMouseLeave;
    }

    public event Action HoverStart
    {
        add => _onHoverStartAction.Add(value);
        remove => _onHoverStartAction.Remove(value);
    }

    public event Action HoverEnd
    {
        add => _onHoverEndAction.Add(value);
        remove => _onHoverEndAction.Remove(value);
    }

    private void HandleMouseEnter()
    {
        _isHovered = true;

        _onHoverStartAction.ForEach(action => action.Invoke());
    }

    private void HandleMouseLeave()
    {
        _isHovered = false;

        _onHoverEndAction.ForEach(action => action.Invoke());
    }
}
