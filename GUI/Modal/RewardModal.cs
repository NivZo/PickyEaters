using Godot;

public partial class RewardModal : Node2D
{
    public static RichTextLabel CoinAmountLabel;
    public static bool IsOpen => _instance != null;
    private static RewardModal _instance;

    public override void _Ready()
    {
        base._Ready();

        CoinAmountLabel = GetNode<RichTextLabel>("Modal/BannerText/CoinAmount");
        _instance = this;
    }

    public static void CloseModal()
    {
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
    }
}
