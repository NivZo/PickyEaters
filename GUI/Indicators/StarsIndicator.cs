using System;
using Godot;

public partial class StarsIndicator : Node2D
{
    private RichTextLabel _label;
    private TextureProgressBar _fg;
    private Node2D _starsProgress;
    private Node2D _notification;

    public override void _Ready()
    {
        base._Ready();

        _notification = GetNode<Node2D>("Notification");
        _notification.Visible = false;
        _starsProgress = GetNode<Node2D>("StarsProgress");
        _label = _starsProgress.GetNode<RichTextLabel>("Text");
        _fg = _starsProgress.GetNode<TextureProgressBar>("Foreground");

        EventManager.ActiveSaveChanged += UpdateLabel;
        EventManager.StarIncreased += UpdateLabel;
        EventManager.StarsCompleted += UpdateLabelOnCompletion;
        UpdateLabel();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventManager.StarIncreased -= UpdateLabel;
        EventManager.ActiveSaveChanged -= UpdateLabel;
        EventManager.StarsCompleted -= UpdateLabelOnCompletion;
    }


    private void UpdateLabel()
    {
        _label.Text = TextUtils.WaveString($"{SaveManager.ActiveSave.CurrentStars} / {StarsManager.RequiredStars} ", amplitude: 12);
        _fg.Value = Math.Min(SaveManager.ActiveSave.CurrentStars, StarsManager.RequiredStars) / (float)StarsManager.RequiredStars * 100;
    }

    private void UpdateLabelOnCompletion()
    {
        // var notificatonLabel = _notification.GetNode<RichTextLabel>("GoldNotification");
        CutsceneManager.Play(new()
        {
            // Show Notification
            // new(() => {
            //     TweenUtils.Pop(_starsProgress, 0, 0.2f, Tween.TransitionType.Cubic);
            //     _notification.Scale = Vector2.Zero;
            //     _notification.Visible = true;
            //     TweenUtils.Pop(_notification, 1, 0.4f);
            //     AudioManager.PlayAudio(AudioType.FoodConsumed);
            // }, 1f),
            new(() => RewardModal.ShowModal(this), 1f),
            // // Add Gold
            // new(() => {
            //     Action<Variant> setLabel = (Variant value) => {
            //         notificatonLabel.Text = $"[right][font gl=15]+{value} [/font][/right]";
            //         if (value.As<float>() > 10) AudioManager.PlayAudio(AudioType.FoodConsumed, 150f / (value.As<float>() * 5 + 150f));
            //     };
            //     TweenUtils.MethodTween(this, setLabel, 150, 0, 3f, Tween.TransitionType.Linear);
            //     CoinsManager.AddCoins(150);
            // }, 1f),
            // new(() => {
            //     TweenUtils.Pop(_notification, 0, 0.2f, Tween.TransitionType.Cubic);
            //     TweenUtils.Pop(_starsProgress, 1, 0.4f, Tween.TransitionType.Elastic);
            //     }, 5f)
        });
    }
}
