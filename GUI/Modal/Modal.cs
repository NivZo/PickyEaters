using Godot;

public partial class Modal : Node2D
{
    private bool _mouseOutside = true;

    public override void _Ready()
    {
        base._Ready();

        GetNode<Area2D>("ClickArea").MouseEntered += () => _mouseOutside = false;
        GetNode<Area2D>("ClickArea").MouseExited += () => _mouseOutside = true;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.IsReleased() && _mouseOutside)
        {
            ModalManager.CloseModal();
        }
    }
}
