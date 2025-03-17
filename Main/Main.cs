using System;
using Godot;

public partial class Main : Node
{
    [Export] bool SaveLocally = false;
    public CanvasLayer ScreenLayer;
    public CanvasLayer ModalLayer;

    public static string ErrorDisplay = string.Empty;

    public override void _Ready()
    {
        base._Ready();

        try
        {
            AudioManager.Setup(GetNode<AudioStreamPlayer>("SoundEffectPlayer"), GetNode<AudioStreamPlayer>("BackgroundMusicPlayer"));

            var transition = GetNode<Transition>("Shaders/Transition");
            ScreenLayer = GetNode<CanvasLayer>("ScreenLayer");
            ModalLayer = GetNode<CanvasLayer>("ModalLayer");
            CutsceneManager.Setup(this);
            ScreenManager.Setup(ScreenLayer, transition);
            ModalManager.Setup(ModalLayer);
            
            SaveManager.SaveLocally = SaveLocally;
            SaveManager.LoadGame();
        }   
        catch (Exception ex)
        {
            GetNode<RichTextLabel>("GUILayer/Exception").Text = $"{ErrorDisplay}\n{ex.Message}";
        }
    }
}
 