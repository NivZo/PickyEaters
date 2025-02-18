using Godot;
using System;

public partial class MainMenu : Node
{

    public override void _Ready()
    {
        base._Ready();

        var _levelLabel = GetNode<RichTextLabel>("CurrentLevel/NumberLabel");
        _levelLabel.Text = TextUtils.WaveString($"{SaveManager.ActiveSave.LevelReached}");
    }
}
