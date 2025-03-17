using Godot;

public partial class DifficultyIndicator : Node2D
{
    private RichTextLabel _label;
    private Sprite2D _bg;
    private int _currentCoinValue = SaveManager.ActiveSave.Coins;

    private static readonly string[] _difficultyName = new[]
    {
        "EASY I",
        "EASY II",
        "EASY III",
        "EASY IV",
        "EASY V",
        "MEDIUM I",
        "MEDIUM II",
        "MEDIUM III",
        "MEDIUM IV",
        "MEDIUM V",
        "HARD I" ,
        "HARD II",
        "HARD III",
        "HARD VI",
        "HARD V",
        "EXPERT I",
        "EXPERT II",
        "EXPERT III",
        "EXPERT VI",
        "EXPERT V",
        "GENIUS I",
        "GENIUS II",
        "GENIUS III",
        "GENIUS VI",
        "GENIUS V",
        "SUPER I",
        "SUPER II",
        "SUPER III",
        "SUPER VI",
        "SUPER V",
        "MASTER I",
        "MASTER II",
        "MASTER III",
        "MASTER VI",
        "MASTER V",
    };

    private static readonly string[] _colors = new[]
    {
        "7390ea", // EASY
        "fec851", // MEDIUM
        "d85058", // HARD
        "ff7dbb", // EXPERT
        "7b51cf", // GENIUS
        "944d12", // SUPER
        "2c2c2c", // MASTER
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
        _label.Text = $"[center][wave amp=12.0 freq=6.0][font gl=5]{_difficultyName[StartingLevel / 15]}[/font][/wave][/center]";
        _bg.SelfModulate = new Color(_colors[StartingLevel / 75]);
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        EventManager.GameLoaded -= Setup;
        EventManager.ActiveSaveChanged -= Setup;
    }
}
