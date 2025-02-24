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
    private float _scrollVelocity = 0;

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
            child.Reparent(_scrollContent);
            Items.Add(child);
        }

        _mask.Size = ScrollAreaShape.Shape.GetRect().Size;
        _mask.PivotOffset = _mask.Size/2;
    }

    // public override void _Process(double delta)
    // {
    //     base._Process(delta);

    //     if (!_scrollAreaPressed && _scrollVelocity != 0)
    //     {
    //         _scrollContent.Position += new Vector2(0, _scrollVelocity / 100);

    //         if (_scrollVelocity > 100)
    //         {
    //             _scrollVelocity -= 100 * (float)delta;
    //         }
    //         else if (_scrollVelocity < -100)
    //         {
    //             _scrollVelocity += 100 * (float)delta;
    //         }
    //         else
    //         {
    //             _scrollVelocity = 0;
    //         }
    //     }
    // }

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
            _scrollContent.Position += new Vector2(0, dragEvent.Relative.Y);
            // _scrollVelocity = dragEvent.Velocity.Y > 0 ? 200 : -200;
        }
    }
}
