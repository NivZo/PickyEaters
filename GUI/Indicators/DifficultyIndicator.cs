using System;
using Godot;

public partial class DifficultyIndicator : Node2D
{
    private RichTextLabel _label;
    private Sprite2D _bg;
    private int _currentCoinValue = SaveManager.ActiveSave.Coins;

    private static readonly string[] _difficultyName = new[]
    {
        "EASY I",
        "MEDIUM I",
        "MEDIUM II",
        "HARD I" ,
        "HARD II",
        "HARD III",
        "HARD IV",
        "EXPERT I",
        "EXPERT II",
        "EXPERT III",
        "EXPERT IV",
        "EXPERT V",
        "GENIUS I",
        "GENIUS II",
        "GENIUS III",
        "GENIUS IV",
        "GENIUS V",
        "GENIUS VI",
        "MASTER I",
        "MASTER II",
        "MASTER III",
        "MASTER IV",
        "MASTER V",
        "MASTER VI",
    };

    public int StartingLevel = -1;

    public override void _Ready()
    {
        base._Ready();

        _label = GetNode<RichTextLabel>("Text");
        _bg = GetNode<Sprite2D>("Background");

        EventManager.GameLoaded += Setup;
        EventManager.ActiveSaveChanged += Setup;
        Setup();
    }

    public void Setup()
    {
        StartingLevel = StartingLevel == -1 ? SaveManager.ActiveSave.LevelReached : StartingLevel;
        _label.Text = $"[center][wave amp=8.0 freq=4.0][font gl=5]{_difficultyName[Math.Max(0, (StartingLevel-1) / 15)]}[/font][/wave][/center]";
        _bg.SelfModulate = LevelManager.GetLevelColor(StartingLevel);
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        EventManager.GameLoaded -= Setup;
        EventManager.ActiveSaveChanged -= Setup;
    }
}
