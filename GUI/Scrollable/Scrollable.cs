using Godot;
using System;
using System.Collections.Generic;

public partial class Scrollable : Area2D
{
    [Export] public CollisionShape2D ScrollAreaShape; 
    public List<Node> Items = new();  
    private Control _mask;
    private Node2D _scrollContent;

    private bool _scrollAreaPressed = false;
    private int _maxScrollDistance = int.MinValue;

    public override void _Ready()
    {
        base._Ready();

        ScrollAreaShape.Reparent(this);
        InputEvent += HandleScroll;
        _mask = GetNode<Control>("ScrollAreaMask");
        _scrollContent = _mask.GetNode<Node2D>("ScrollContent");

        foreach (var child in GetChildren())
        {
            if (child == ScrollAreaShape || child == _mask) { continue; }
            AddChildToScrollableContent(child);
        }

        _mask.Size = ScrollAreaShape.Shape.GetRect().Size;
        _mask.PivotOffset = _mask.Size/2;
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (_scrollAreaPressed && @event is InputEventMouseButton inputEventMouseButton
            && inputEventMouseButton.ButtonIndex == MouseButton.Left)
        {
            if (inputEventMouseButton.IsReleased())
            {
                _scrollAreaPressed = false;
            }
        }
    }

    public void AddChildToScrollableContent(Node child)
    {
        if (child.GetParent() == null)
        {
            _scrollContent.AddChild(child);
        }
        else
        {
            child.Reparent(_scrollContent);
        }

        Items.Add(child);

        if (child is Node2D child2D)
        {
            _maxScrollDistance = Math.Max(_maxScrollDistance, (int)child2D.Position.Y);
        }
        if (child is Control childControl)
        {
            _maxScrollDistance = Math.Max(_maxScrollDistance, (int)childControl.Position.Y);
        }
    }

    public void ScrollTo(float yDelta)
    {
        Action<Variant> scrollAction = yd => SafeScroll(yd.As<float>());
        TweenUtils.MethodTween(this, scrollAction, _scrollContent.Position.Y, _scrollContent.Position.Y + yDelta, .5f);
    }

    private void SafeScroll(float yPos)
    {
        yPos = Math.Min(0, yPos);
        yPos = Math.Max(1500 - _maxScrollDistance, yPos);
        _scrollContent.Position = new(0, yPos);
    }

    private void HandleScroll(Node viewport, InputEvent inputEvent, long shapeIdx)
    {
        if (inputEvent is InputEventMouseButton mouseButtonEvent)
        {
            if (mouseButtonEvent.IsReleased())
            {
                _scrollAreaPressed = false;
            }

            else if (mouseButtonEvent.IsPressed())
            {
                _scrollAreaPressed = true;
            }
        }

        if (_scrollAreaPressed && inputEvent is InputEventMouseMotion dragEvent)
        {
            SafeScroll(_scrollContent.Position.Y + dragEvent.Relative.Y);
            // _scrollContent.Position += new Vector2(0, dragEvent.Relative.Y);
        }
    }
}
