using System;
using Godot;

public abstract partial class CustomButtonBase : Button
{
    [Export] public Texture2D CustomIcon;
    [Export] public float Cooldown = 0f;
    [Export] public AudioType Sound = AudioType.None;

    protected Func<bool> IsEnabledFunc = () => true;

    protected TextureRect _icon;
    protected TextureRect _iconShadow;

    private Timer _cooldownTimer;

    private bool _isMouseOn = false;

    public override void _Ready()
    {
        base._Ready();

        ButtonDown += HandleButtonDownInternal;
        ButtonUp += HandleButtonUpInternal;
        MouseExited += () => _isMouseOn = false;

        if (Cooldown > 0)
        {
            _cooldownTimer = new() { WaitTime = Cooldown, OneShot = true, Autostart = false };
            AddChild(_cooldownTimer);
        }
    
        _icon = GetNode<TextureRect>("Icon");
        _icon.Texture = CustomIcon;
        _iconShadow = GetNode<TextureRect>("IconShadow");
        _iconShadow.Texture = CustomIcon;

        PivotOffset = Size/2;
    }

    private void OnClickInternal()
    {
        if (!IsEnabled()) return;
        if (Sound != AudioType.None) AudioManager.PlayAudio(Sound);
        OnClick();
    }

    private void HandleButtonDownInternal()
    {
        if (IsEnabled())
        {
            _isMouseOn = true;
            Modulate = new Color(.8f, .8f, .8f);
            HandleButtonDown();
            AudioManager.PlayAudio(AudioType.ButtonPress);
            Input.VibrateHandheld(100, (float)SaveManager.ActiveSave.ScreenShakeStrength * 0.05f);
        }
    }

    private void HandleButtonUpInternal()
    {
        if (IsEnabled())
        {
            Modulate = new Color(1, 1, 1);
            HandleButtonUp();
            AudioManager.PlayAudio(AudioType.ButtonRelease);
            Input.VibrateHandheld(100, (float)SaveManager.ActiveSave.ScreenShakeStrength * 0.05f);

            if (_isMouseOn)
            {
                OnClickInternal();
            }
            _isMouseOn = false;

            if (Cooldown > 0)
            {
                _cooldownTimer.Start();
            }
        }
    }

    protected virtual bool IsEnabled() => (_cooldownTimer == null || _cooldownTimer.TimeLeft == 0) && ActionManager.IsActionAvailable() && IsEnabledFunc();
    
    protected virtual void OnClick(){}
    protected abstract void HandleButtonDown();
    protected abstract void HandleButtonUp();
}
