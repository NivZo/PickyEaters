using System;
using Godot;

public partial class CoinsIndicator : Node2D
{
    private RichTextLabel _label;
    private int _currentCoinValue = SaveManager.ActiveSave.Coins;

    public override void _Ready()
    {
        base._Ready();

        _label = GetNode<RichTextLabel>("Text");

        EventManager.GameLoaded += Setup;
        EventManager.ActiveSaveChanged += UpdateLabel;
    }

    private void SetLabel(Variant value) => _label.Text = TextUtils.WaveString(value.ToString(), amplitude: 12);

    private void Setup()
    {
        _currentCoinValue = SaveManager.ActiveSave.Coins;
        SetLabel(_currentCoinValue);
    }

    private void UpdateLabel()
    {
        var target = SaveManager.ActiveSave.Coins;
        if (target != _currentCoinValue)
        {
            TweenUtils.MethodTween(this, SetLabel, _currentCoinValue, target, 3f, Tween.TransitionType.Linear);
            _currentCoinValue = target;
        }
    }
}
