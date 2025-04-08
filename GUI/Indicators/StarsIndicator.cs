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
        CutsceneManager.Play(new()
        {
            new(() => {
                RewardModal.ShowModal(this);
                AcceptRewardButton.SetReward(CoinsManager.QuestReward);
            }, 1f),
        });
    }
}
