using Godot;
using System;

public partial class MainMenu : Node
{
    public override void _EnterTree()
    {
        base._EnterTree();

        var _levelLabel = GetNode<RichTextLabel>("CurrentLevel/NumberLabel");
        _levelLabel.Text = TextUtils.WaveString($"{SaveManager.ActiveSave.LevelReached}", letterDistance: 25);

        BackgroundManager.ChangeColor(NamedColor.LightBlue.GetColor(), lightenFactor: 0);
    }
}
