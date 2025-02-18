using System;
using Godot;

public abstract partial class CustomButton : CustomButtonBase
{
    [Export] public string CustomText = string.Empty;
    [Export] public Color Color;
    [Export] public bool WaveString = true;
    [Export] public bool Highlight = true;

    private TextureRect _bg;
    private TextureRect _pressedBg;
    private RichTextLabel _textLabel;
    private Vector2 _pressOffset = new(0, 28);

    public override void _Ready()
    {
        base._Ready();

        _bg = GetNode<TextureRect>("Background");
        _pressedBg = GetNode<TextureRect>("PressedBackground");
        _textLabel = GetNode<RichTextLabel>("Text");
        if (CustomText != string.Empty)
        {
            _textLabel.Text = WaveString ? TextUtils.WaveString(CustomText) : TextUtils.AddAttribute(CustomText, "center");
        }
        else
        {
            _textLabel.Text = string.Empty;
        }
        if (Color != new Color())
        {
            _bg.SelfModulate = Color;
            _pressedBg.SelfModulate = Color;
        }
        if (!Highlight)
        {
            _textLabel.Material = null;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!IsEnabledFunc())
        {
            Modulate = Modulate with { A = .5f };
        }
        else
        {
            Modulate = Modulate with { A = 1 };
        }
    }


    protected override void HandleButtonDown()
    {
        _bg.Visible = false;
        _pressedBg.Visible = true;
        _textLabel.Position += _pressOffset;
        _icon.Position += _pressOffset/2;
        _iconShadow.Position += _pressOffset/2;
    }

    protected override void HandleButtonUp()
    {
        _bg.Visible = true;
        _pressedBg.Visible = false;
        _textLabel.Position -= _pressOffset;
        _icon.Position -= _pressOffset/2;
        _iconShadow.Position -= _pressOffset/2;
    }
}
