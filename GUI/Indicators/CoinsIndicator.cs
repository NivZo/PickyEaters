using System;
using Godot;

public partial class CoinsIndicator : Node2D
{
    private RichTextLabel _label;
    private int _currentCoinValue = 0;

    public override void _Ready()
    {
        base._Ready();

        _label = GetNode<RichTextLabel>("Text");

        SignalProvider.Instance.ActiveSaveChanged += UpdateLabel;
    }

    private void UpdateLabel()
    {
        var target = SaveManager.ActiveSave.Coins;
        Action<Variant> setLabel = (Variant value) => _label.Text = TextUtils.WaveString(value.ToString(), amplitude: 12);
        TweenUtils.MethodTween(this, setLabel, _currentCoinValue, target, 1f);
        _currentCoinValue = target;
    }
}
