using Godot;

public partial class SignalProvider : Node
{
    [Signal] public delegate void ActiveSaveChangedEventHandler();
    [Signal] public delegate void LevelResetEventHandler();
    [Signal] public delegate void FadeOutTranistionFinishedEventHandler();
    [Signal] public delegate void MoveSelectionStartedEventHandler(Vector2I EaterPosId, Vector2I PossibleFoodPosId, bool IsCurrentlySelected);
    [Signal] public delegate void MoveSelectionCancelledEventHandler(Vector2I EaterPosId);
    [Signal] public delegate void MovePerformedEventHandler(Vector2I EaterPosId, Vector2I FoodPosId, FoodType FoodType, bool IsLast, bool IsHint);

    public static SignalProvider Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        Instance = this;
    }

    public static void Emit(StringName signalName, params Variant[] args) => Instance.EmitSignal(signalName, args);
}