using Godot;
using System;
using System.Collections.Generic;

public partial class VictoryModal : Node2D
{
    public override void _Ready()
    {
        base._Ready();
        PlayCutscene();
    }

    private void PlayCutscene()
    {
        var rewardPerStar = 5 * Math.Max(1, LevelManager.CurrentLevelId / 15);

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

            TweenUtils.MethodTween(coinsLabel, value => coinsLabel.Text = TextUtils.WaveString($"+{value}"), (starNumber-1) * rewardPerStar, starNumber * rewardPerStar, .4f);
            coinsLabel.Scale = new(.8f, .8f);
            TweenUtils.Pop(coinsLabel, 1, .4f);
        };

        var cutscenes = new List<CutsceneManager.CutsceneAction>()
        {
            new(PopStar(GetNode<Sprite2D>("Modal/StarL"), 1.2f), 1f),
            new(IncrementGold(1), 0.05f),
            new(PopStar(GetNode<Sprite2D>("Modal/StarM"), 1.4f), .6f),
            new(IncrementGold(2), 0.05f),
        };
        var reward = 2 * rewardPerStar;

        if (LevelManager.IsFlawlessVictory())
        {
            cutscenes.Add(new(PopStar(GetNode<Sprite2D>("Modal/StarR"), 1.2f), .6f));
            cutscenes.Add(new(IncrementGold(3), 0.05f));
            reward += rewardPerStar;

            SaveManager.ActiveSave.LevelStarsObtained[LevelManager.CurrentLevelId] = 3;
        }
        else
        {
            SaveManager.ActiveSave.LevelStarsObtained[LevelManager.CurrentLevelId] = Math.Max(2, SaveManager.ActiveSave.LevelStarsObtained[LevelManager.CurrentLevelId]);
        }

        cutscenes.Add(new(() => CoinsManager.AddCoins(reward), 0.05f));

        CutsceneManager.Play(cutscenes);
    }
}
