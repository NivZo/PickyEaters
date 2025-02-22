using Godot;

public partial class Collection : Node
{
    private Area2D _scrollArea;
    private Node2D _scrollContent;

    private bool _scrollAreaPressed = false;

    public override void _Ready()
    {
        base._Ready();

        _scrollArea = GetNode<Area2D>("ScrollArea");
        _scrollArea.InputEvent += HandleScroll;
        _scrollContent = GetNode<Node2D>("ScrollAreaMask/ScrollContent");
    }

    private void HandleScroll(Node viewport, InputEvent inputEvent, long shapeIdx)
    {
        if (inputEvent is InputEventMouseButton mouseButtonEvent)
        {
            GD.Print("Click");
            if (mouseButtonEvent.IsReleased())
            {
                GD.Print("Scroll end");
                _scrollAreaPressed = false;
            }

            else if (mouseButtonEvent.IsPressed())
            {
                GD.Print("Scroll start");
                _scrollAreaPressed = true;
            }
        }

        if (_scrollAreaPressed && inputEvent is InputEventMouseMotion dragEvent)
        {
            GD.Print(dragEvent.Relative);
            _scrollContent.Position += new Vector2(0, dragEvent.Relative.Y);
        }
    }
}
