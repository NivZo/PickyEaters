using System;
using Godot;

public abstract partial class CustomIconButton : CustomButtonBase
{
    private Vector2 _baseIconPosition = Vector2.Zero;
    private Vector2 _pressOffset = new(0, 28);
    private readonly Vector2 _shadowOffset = new(0, 8);

        public override void _Ready()
    {
        base._Ready();

        _baseIconPosition = _icon.Position;
    }

    protected override void HandleButtonDown()
    {
        _icon.Position = _baseIconPosition + _pressOffset/2;
        _iconShadow.Position = _icon.Position + _shadowOffset;
    }

    protected override void HandleButtonUp()
    {
        _icon.Position = _baseIconPosition;
        _iconShadow.Position = _icon.Position + _shadowOffset;
    }
}
