using System;
using Godot;

public abstract partial class CustomIconButton : CustomButtonBase
{
    private Vector2 _pressOffset = new(0, 6);

    protected override void HandleButtonDown()
    {
        _icon.Position += _pressOffset;
    }

    protected override void HandleButtonUp()
    {
        _icon.Position -= _pressOffset;
    }
}
