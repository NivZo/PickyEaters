using Godot;

public partial class RewardModal : Node2D
{
    private ColorRect _fade;
    public static RichTextLabel CoinAmountLabel;
    public static bool IsOpen => _instance != null;
    private static RewardModal _instance;

    public override void _Ready()
    {
        base._Ready();
        CoinAmountLabel = GetNode<RichTextLabel>("Modal/BannerText/CoinAmount");
        _fade = GetNode<ColorRect>("Fade");
        _instance = this;
        EnableFade();
    }

    public static void CloseModal()
    {
        _instance.DisableFade();
        CutsceneManager.Play(new() {
            new(() => TweenUtils.Travel(_instance, SizeUtils.ScreenCenter + new Vector2(0, SizeUtils.ScreenH), 0.4f, Tween.TransitionType.Sine), 0),
            new(() => { _instance.QueueFree(); _instance = null; }, 0.4f)
        });
    }

    public static void ShowModal(Node parent)
    {
        var modal = GD.Load<PackedScene>("res://GUI/Modal/RewardModal.tscn").Instantiate<RewardModal>();
        modal.GlobalPosition = SizeUtils.ScreenCenter + new Vector2(0, SizeUtils.ScreenH);
        parent.AddChild(modal);
        modal.GlobalPosition = SizeUtils.ScreenCenter + new Vector2(0, SizeUtils.ScreenH);
        TweenUtils.Travel(modal, SizeUtils.ScreenCenter, 0.4f, Tween.TransitionType.Sine);
        AudioManager.PlayAudio(AudioType.FoodConsumed);
    }

    public void EnableFade()
    {
        _fade.SelfModulate = _fade.SelfModulate with { A = 0 };
        _fade.Visible = true;
        TweenUtils.MethodTween(_fade, alpha => _fade.SelfModulate = _fade.SelfModulate with { A = (float)alpha/255 }, 0, 255, 3);
    }

    public void DisableFade()
    {
        _fade.Visible = false;
        TweenUtils.MethodTween(_fade, alpha => _fade.SelfModulate = _fade.SelfModulate with { A = (float)alpha/255 }, 255, 0, 1);
    }
}
