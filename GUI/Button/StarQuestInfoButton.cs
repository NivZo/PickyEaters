using Godot;
using System;

public partial class StarQuestInfoButton : Button
{
    public override void _Ready()
    {
        base._Ready();
        Pressed += ModalManager.OpenStarQuestInfoModal;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Pressed -= ModalManager.OpenStarQuestInfoModal;
    }

}
