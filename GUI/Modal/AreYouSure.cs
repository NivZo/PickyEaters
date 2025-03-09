using System;
using Godot;

public partial class AreYouSure : Node2D
{
    private static Action _onConfirm;
    private static string _text;

    private AreYouSureConfirmButton _confirmButton;
    private RichTextLabel _textLabel;
    private Vector2 _textLabelOffset = new(0, 32);
    private bool _mouseOutside = true;

    public override void _Ready()
    {
        base._Ready();

        _confirmButton = GetNode<AreYouSureConfirmButton>("Modal/Confirm");
        _confirmButton.OnConfirm = _onConfirm;
        _textLabel = GetNode<RichTextLabel>("Modal/BannerText");
        _textLabel.Text = TextUtils.WaveString(_text);

        if (!_text.Contains('\n'))
        {
            _textLabel.Position += _textLabelOffset;
        }

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

    public static void SetText(string text) => _text = text;
    public static void SetOnConfirm(Action onConfirm) => _onConfirm = () => { onConfirm(); ModalManager.CloseModal(); };
}
