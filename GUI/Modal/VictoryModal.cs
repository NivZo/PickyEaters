using Godot;
using System;

public partial class VictoryModal : Node2D
{
    public override void _Ready()
    {
        base._Ready();
        PlayCutscene();
    }

    private void PlayCutscene()
    {
        Action PopStar(Sprite2D star, float scale) => () =>
        {
            star.Scale = Vector2.Zero;
            star.Visible = true;
            TweenUtils.Pop(star, scale);
            AudioManager.PlayAudio(AudioType.FoodConsumed);
        };

        Action IncrementGold(int starNumber) => () =>
        {
            var rewardsLabel = GetNode<RichTextLabel>("Modal/RewardsLabel");
            var coinsLabel = GetNode<RichTextLabel>("Modal/CoinsValueLabel");

            rewardsLabel.Scale = new(.8f, .8f);
            TweenUtils.Pop(rewardsLabel, 1, .4f);

            TweenUtils.MethodTween(coinsLabel, value => coinsLabel.Text = TextUtils.WaveString($"+{value}"), (starNumber-1) * 25, starNumber * 25, .4f);
            coinsLabel.Scale = new(.8f, .8f);
            TweenUtils.Pop(coinsLabel, 1, .4f);
        };

        CutsceneManager.Play(new() {
            new(PopStar(GetNode<Sprite2D>("Modal/StarL"), 1.2f), 1.2f),
            new(IncrementGold(1), 0.05f),
            new(PopStar(GetNode<Sprite2D>("Modal/StarM"), 1.4f), .6f),
            new(IncrementGold(2), 0.05f),
            new(PopStar(GetNode<Sprite2D>("Modal/StarR"), 1.2f), .6f),
            new(IncrementGold(3), 0.05f),
            new(() => CoinsManager.AddCoins(75), 0.05f)
        });
    } 

}
