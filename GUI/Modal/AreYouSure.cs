using System;
using Godot;

public partial class AreYouSure : Node2D
{
    private static Action _onConfirm;

    private AreYouSureConfirmButton _confirmButton;

    private bool _mouseOutside = true;

    public override void _Ready()
    {
        base._Ready();

        _confirmButton = GetNode<AreYouSureConfirmButton>("Modal/Confirm");
        _confirmButton.OnConfirm = _onConfirm;
        GetNode<Area2D>("Modal/ClickArea").MouseEntered += () => _mouseOutside = false;
        GetNode<Area2D>("Modal/ClickArea").MouseExited += () => _mouseOutside = true;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event is InputEventMouseButton && _mouseOutside)
        {
            ModalManager.CloseModal();
        }
    }

    public static void SetOnConfirm(Action onConfirm) => _onConfirm = () => { onConfirm(); ModalManager.CloseModal(); };
}
