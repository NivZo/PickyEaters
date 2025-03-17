using Godot;
using System;
using System.Collections.Generic;

public partial class VictoryModal : Node2D
{
    private RichTextLabel _progressLabel;
    private RichTextLabel _descriptionLabel;
    private StarsIndicator _starsIndicator;
    private Sprite2D _starL;
    private Sprite2D _starM;
    private Sprite2D _starR;

    public override void _Ready()
    {
        base._Ready();

        _progressLabel = GetNode<RichTextLabel>("Modal/ProgressLabel");
        _descriptionLabel = GetNode<RichTextLabel>("Modal/DescriptionLabel");
        _starsIndicator = GetNode<StarsIndicator>("Modal/StarsIndicator");
        _starL = GetNode<Sprite2D>("Modal/StarL");
        _starM = GetNode<Sprite2D>("Modal/StarM");
        _starR = GetNode<Sprite2D>("Modal/StarR");

        PlayCutscene();
    }

    private Action CreateAddStarAction(Sprite2D star, int starNumber, float scale, string desc, bool shouldIncreaseStars)
    {
        star.Visible = !shouldIncreaseStars;

        return () => {
            if (shouldIncreaseStars)
            {
                _progressLabel.Scale = new(.8f, .8f);
                TweenUtils.Pop(_progressLabel, 1, .4f);

                _starsIndicator.Scale = new(.8f, .8f);
                TweenUtils.Pop(_starsIndicator, 1, .4f);
                StarsManager.AddStar(LevelManager.CurrentLevelId);
                
                star.Scale = Vector2.Zero;
            }
            else
            {
                star.Scale = new(.8f, .8f);
            }

            star.Visible = true;
            TweenUtils.Pop(star, scale);

            _descriptionLabel.Text = $"{_descriptionLabel.Text}[font gl=15]{desc}[/font]";
            _descriptionLabel.Scale = new(.8f, .8f);
            TweenUtils.Pop(_descriptionLabel, 1, .4f);

            AudioManager.PlayAudio(AudioType.FoodConsumed, 1 + 0.25f * starNumber);
        };
    }
        

    private void PlayCutscene()
    {
        _descriptionLabel.Text = string.Empty;
        var currStars = SaveManager.ActiveSave.LevelStarsObtained[LevelManager.CurrentLevelId];
        var nextStar = _starM;
        var cutscenes = new List<CutsceneManager.CutsceneAction>()
        {
            new(CreateAddStarAction(_starL, 1, 1.2f, " •COLOR 100%", currStars == 0), 1f),
        };

        if (LevelManager.IsFlawlessVictory())
        {
            cutscenes.Add(new(CreateAddStarAction(nextStar, 2, 1.4f, "\n •WHITE 100%", currStars < 2), .6f));
            nextStar = _starR;
        }

        if (HistoryManager.UndoCount >= 0)
        {
            cutscenes.Add(new(CreateAddStarAction(nextStar, nextStar == _starR ? 3 : 2, 1.2f, "\n •UNDO LIMIT", currStars < 3), .6f));
        }

        CutsceneManager.Play(cutscenes);
    }
}
