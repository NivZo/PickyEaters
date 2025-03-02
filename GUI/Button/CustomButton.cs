using System;
using Godot;

public abstract partial class CustomButton : CustomButtonBase
{
    [Export] public string CustomText = string.Empty;
    [Export] public int CustomTextSize = 140;
    [Export] public Color Color;
    [Export] public bool WaveString = true;
    [Export] public bool Highlight = true;

    private TextureRect _bg;
    private TextureRect _pressedBg;
    private RichTextLabel _textLabel;
    private Vector2 _baseIconPosition = Vector2.Zero;
    private Vector2 _pressOffset = new(0, 28);
    private readonly Vector2 _shadowOffset = new(0, 8);

    public override void _Ready()
    {
        base._Ready();

        _baseIconPosition = _icon.Position;

        _bg = GetNode<TextureRect>("Background");
        _pressedBg = GetNode<TextureRect>("PressedBackground");
        _textLabel = GetNode<RichTextLabel>("Text");
        if (CustomText != string.Empty)
        {
            _textLabel.Text = WaveString ? TextUtils.WaveString(CustomText) : TextUtils.AddAttribute(CustomText, "center");
            _textLabel.AddThemeFontSizeOverride("normal_font_size", CustomTextSize);
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
        if (!IsEnabled())
        {
            Modulate = Modulate with { A = .5f };
            _textLabel.AddThemeConstantOverride("outline_size", 0);
        }
        else
        {
            Modulate = Modulate with { A = 1 };
            _textLabel.RemoveThemeConstantOverride("outline_size");
        }
    }


    protected override void HandleButtonDown()
    {
        _bg.Visible = false;
        _pressedBg.Visible = true;
        _textLabel.Position += _pressOffset * (CustomTextSize / 140f);
        _icon.Position = _baseIconPosition + _pressOffset/2;
        _iconShadow.Position = _icon.Position + _shadowOffset;
    }

    protected override void HandleButtonUp()
    {
        _bg.Visible = true;
        _pressedBg.Visible = false;
        _textLabel.Position -= _pressOffset * (CustomTextSize / 140f);
        _icon.Position = _baseIconPosition;
        _iconShadow.Position = _icon.Position + _shadowOffset;
    }
}
