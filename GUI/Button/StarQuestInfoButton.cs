using Godot;
using System;

public partial class StarQuestInfoButton : Button
{
    public override void _Ready()
    {
        base._Ready();
        Pressed += OnPress;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Pressed -= OnPress;
    }

    private void OnPress()
    {
        AudioManager.PlayAudio(AudioType.Undo);
        ModalManager.OpenStarQuestInfoModal();
    }
}
