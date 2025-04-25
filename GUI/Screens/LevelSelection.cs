using System;
using System.Collections.Generic;
using Godot;

public partial class LevelSelection : PagedScreen<PlaySelectedLevelButton>
{
    private const int ITEMS_PER_PAGE = 15;
    private DifficultyIndicator _difficultyIndicator;

    public override void _Ready()
    {
        _difficultyIndicator = GetNode<DifficultyIndicator>("GUILayer/DifficultyIndicator");
        CurrentPage = Mathf.CeilToInt(SaveManager.ActiveSave.LevelReached / (float)ITEMS_PER_PAGE) - 1;
        base._Ready();
    }

    protected override List<PlaySelectedLevelButton> CreateContents(int pageId)
    {

        var minLevel = Math.Max(1, pageId*ITEMS_PER_PAGE + 1);
        var maxLevel = Math.Min(LevelManager.MaxLevel, (pageId+1)*ITEMS_PER_PAGE);
        var buttons = new List<PlaySelectedLevelButton>();

        for (int i = minLevel; i <= maxLevel; i++)
        {
            var lvlBtn = GD.Load<PackedScene>("res://GUI/Button/PlaySelectedLevelButton.tscn").Instantiate<PlaySelectedLevelButton>();
            lvlBtn.LevelId = i;
            lvlBtn.SetAnchorsPreset(Control.LayoutPreset.TopLeft);
            lvlBtn.Size = new(360, 220);
            var x = (i%3) switch
                {
                    1 => 100,
                    2 => 540,
                    _ => 980,
                };
            lvlBtn.Position = new(x, 400 + 352 * Mathf.FloorToInt((i-minLevel)/3));

            buttons.Add(lvlBtn);
        }

        return buttons;
    }

    protected override int GetPageCount() => Mathf.CeilToInt(LevelManager.MaxLevel / (float)ITEMS_PER_PAGE);

    protected override void OnPageUpdate(int newPageId)
    {
        _difficultyIndicator.StartingLevel = newPageId * ITEMS_PER_PAGE + 1;
        _difficultyIndicator.Setup();

        BackgroundManager.ChangeColor(LevelManager.GetLevelColor(_difficultyIndicator.StartingLevel));
    }
}
