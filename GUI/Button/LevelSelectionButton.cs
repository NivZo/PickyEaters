using Godot;

public partial class LevelSelectionButton : Button
{
    public override void _Ready()
    {
        base._Ready();
        Pressed += OnPress;
        ButtonDown += OnButtonDown;
        ButtonUp += OnButtonUp;

        var backgroundOverlay = GetNode<TextureRect>("BackgroundOverlay");
        // var backgroundOverlayOutline = GetNode<TextureRect>("BackgroundOverlayOutline");
        var color = LevelManager.GetLevelColor(SaveManager.ActiveSave.LevelReached).Lightened(.3f);
        SetTextureRectColor(backgroundOverlay, color);
        // SetTextureRectColor(backgroundOverlayOutline, color with { A = 0.7f });
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
        
    private void SetTextureRectColor(TextureRect textureRect, Color color)
    {
        if (textureRect.Texture is GradientTexture2D gradientTexture)
        {
            gradientTexture.Gradient.SetColor(0, color);
            gradientTexture.Gradient.SetColor(1, color.Darkened(.15f));
        }
    }
}
