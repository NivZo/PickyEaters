using Godot;

public partial class SignalProvider : Node
{
    [Signal]
    public delegate void CoinsValueChangedEventHandler();

    public static SignalProvider Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        Instance = this;
    }

    public static void Emit(StringName signalName) => Instance.EmitSignal(signalName);
}