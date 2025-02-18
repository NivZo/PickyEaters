using Godot;

public partial class Main : Node
{
    [Export] bool SaveLocally = false;
    public CanvasLayer ScreenLayer;
    public CanvasLayer ModalLayer;

    public override void _Ready()
    {
        base._Ready();
        
        AudioManager.Instance.Setup(GetNode<AudioStreamPlayer>("SoundEffectPlayer"), GetNode<AudioStreamPlayer>("BackgroundMusicPlayer"));
        SaveManager.SaveLocally = SaveLocally;
        SaveManager.LoadGame();
        SignalProvider.Emit(SignalProvider.SignalName.CoinsValueChanged);

        ScreenLayer = GetNode<CanvasLayer>("ScreenLayer");
        ModalLayer = GetNode<CanvasLayer>("ModalLayer");
        ScreenManager.Instance.Setup(ScreenLayer);
        ScreenManager.Instance.TransitionToScreen(ScreenManager.ScreenType.MainMenu);
        ModalManager.Instance.Setup(ModalLayer);
    }
}
