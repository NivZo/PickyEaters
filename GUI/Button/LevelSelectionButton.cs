using Godot;

public partial class LevelSelectionButton : Button
{
    public override void _Ready()
    {
        base._Ready();
        Pressed += OnPress;
        ButtonDown += OnButtonDown;
        ButtonUp += OnButtonUp;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Pressed -= OnPress;
    }

    private void OnButtonDown()
    {
        Modulate = new("c8c8c8");
        Scale = new Vector2(0.95f, 0.95f);
    }

    private void OnButtonUp()
    {
        Modulate = new("ffffff");
        Scale = Vector2.One;
    }

    private void OnPress()
    {
        if (ModalManager.CurrentOpenModal == ModalManager.ModalType.None)
        {
            AudioManager.PlaySoundEffect(AudioType.Undo);
            ScreenManager.TransitionToScreen(ScreenManager.ScreenType.LevelSelection);
            OnButtonUp();
        }
    }
}
