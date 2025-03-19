using System.Linq;
using Godot;

public partial class TotalStarsIndicator : Node2D
{
    private RichTextLabel _label;

    public override void _Ready()
    {
        base._Ready();
        _label = GetNode<RichTextLabel>("Text");

        EventManager.ActiveSaveChanged += UpdateLabel;
        UpdateLabel();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventManager.ActiveSaveChanged -= UpdateLabel;
    }


    private void UpdateLabel()
    {
        _label.Text = TextUtils.WaveString($"[left]{SaveManager.ActiveSave.LevelStarsObtained.Sum()} / {LevelManager.MaxLevel * 3}[/left]", amplitude: 12, frequency: 2, center: false);
    }
}
