using Godot;

public partial class PlayScreen : Node
{
    public override void _Ready()
    {
        base._Ready();

        var gameLayer = GetNode<CanvasLayer>("GameLayer");
        LevelManager.Instance.Setup(gameLayer);
        LevelManager.Instance.LoadLevel(SaveManager.ActiveSave.LevelReached);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (ModalManager.Instance.CurrentOpenModal == ModalManager.ModalType.None && LevelManager.Instance.IsVictory())
        {
            ModalManager.Instance.OpenVictoryModal();
            
            CoinsManager.Instance.AddCoins(100);
            SignalProvider.Emit(SignalProvider.SignalName.CoinsValueChanged);
        }
    }
}
