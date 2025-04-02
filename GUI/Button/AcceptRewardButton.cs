using System;
using Godot;

public partial class AcceptRewardButton : CustomButton
{
    private static int _reward = 200;
    
    public override void _Ready()
    {
        base._Ready();
        _reward = CoinsManager.QuestReward;
    }

    public static void SetReward(int value)
    {
        _reward = value;
        RewardModal.CoinAmountLabel.Text = $"[right][wave amp=20.0 freq=8.0 connected=1][font gl=15]+{value} [/font][/wave][/right]";
    }
    
    protected override void OnClick()
    {
        CutsceneManager.Play(new() {
            // Add Gold
            new(() => {
            Action<Variant> setLabel = (Variant value) => {
                    RewardModal.CoinAmountLabel.Text = $"[right][wave amp=20.0 freq=8.0 connected=1][font gl=15]+{value} [/font][/wave][/right]";
                    if (value.As<float>() > 10) AudioManager.PlayAudio(AudioType.FoodConsumed, _reward / (value.As<float>() * 5 + _reward));
                };
                TweenUtils.MethodTween(this, setLabel, _reward, 0, 3f, Tween.TransitionType.Linear);
                CoinsManager.AddCoins(_reward);
            }, 1f),
            // Close Modal
            new(RewardModal.CloseModal, 5f),
        });
    }

}
