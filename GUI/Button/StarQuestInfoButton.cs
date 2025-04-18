using Godot;
using System;

public partial class StarQuestInfoButton : Button
{
    public override void _Ready()
    {
        base._Ready();
        Pressed += OnPress;
        MouseEntered += OnHover;
        MouseExited += OnUnhover;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Pressed -= OnPress;
    }

    private void OnHover()
    {
        Modulate = new("c8c8c8");
        Scale = new Vector2(0.95f, 0.95f);
    }

    private void OnUnhover()
    {
        Modulate = new("ffffff");
        Scale = Vector2.One;
    }

    private void OnPress()
    {
        if (ModalManager.CurrentOpenModal == ModalManager.ModalType.None)
        {
            AudioManager.PlayAudio(AudioType.Undo);
            ModalManager.OpenStarQuestInfoModal();
        }
    }
}
