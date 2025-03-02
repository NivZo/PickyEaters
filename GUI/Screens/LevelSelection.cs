using Godot;

public partial class LevelSelection : Node
{
    private Scrollable _scrollable;

    public override void _Ready()
    {
        base._Ready();
        
        _scrollable = GetNode<Scrollable>("GUILayer/Scrollable");

        var levelReached = SaveManager.ActiveSave.LevelReached;

        for (int i = 1; i <= levelReached; i++)
        {
            var lvlBtn = GD.Load<PackedScene>("res://GUI/Button/PlaySelectedLevelButton.tscn").Instantiate<PlaySelectedLevelButton>();
            lvlBtn.LevelId = i;
            lvlBtn.SetAnchorsPreset(Control.LayoutPreset.Center);
            lvlBtn.Size = new(324, 220);
            var x = (i%3) switch
                {
                    1 => 72,
                    2 => 560,
                    _ => 1048,
                };
            lvlBtn.Position = new(x, 72 + 300 * ((i-1)/3));

            _scrollable.AddChildToScrollableContent(lvlBtn);
        }

        _scrollable.ScrollTo(1800 - 300 * ((levelReached-1)/3));
    }
}
