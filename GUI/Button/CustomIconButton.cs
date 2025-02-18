using System;
using Godot;

public abstract partial class CustomIconButton : CustomButtonBase
{
    private Vector2 _pressOffset = new Vector2(0, 6);

    public override void _Process(double delta)
    {
        base._Process(delta);
        // if (!IsEnabled())
        // {
        //     _bg.SelfModulate = _bg.SelfModulate with { A = .5f };
        // }
        // else
        // {
        //     _bg.SelfModulate = _bg.SelfModulate with { A = 1 };
        // }
    }


    protected override void HandleButtonDown()
    {
        _icon.Position += _pressOffset;
    }

    protected override void HandleButtonUp()
    {
        _icon.Position -= _pressOffset;
    }
}
