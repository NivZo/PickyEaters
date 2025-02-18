using System;
using System.Linq;
using Godot;

public class ModalManager
{
    private static ModalType[] UNCLOSABLE_MODALS = new ModalType[] { ModalType.Victory };
    public static ModalManager Instance { get; } = new ModalManager();

    private Node2D _currentModal = null;
    private CanvasLayer _modalLayer;

    public ModalType CurrentOpenModal = ModalType.None;

    public void Setup(CanvasLayer modalLayer)
    {
        _modalLayer = modalLayer;
    }
    
    public void OpenVictoryModal()
    {
        LevelManager.Instance.IncreaseLevelReached();

        _currentModal = GD.Load<PackedScene>("res://GUI/Modal/VictoryModal.tscn").Instantiate<Node2D>();
        CurrentOpenModal = ModalType.Victory;

        OpenModal();
    }

    public void OpenSettingsModal()
    {
        _currentModal = GD.Load<PackedScene>("res://GUI/Modal/SettingsModal.tscn").Instantiate<Node2D>();
        CurrentOpenModal = ModalType.Settings;

        OpenModal();
    }

    public void CloseModal(bool overideUnclosable = false)
    {
        if (_currentModal != null && (overideUnclosable || !UNCLOSABLE_MODALS.Contains(CurrentOpenModal)))
        {
            _currentModal.QueueFree();
            _currentModal = null;
            CurrentOpenModal = ModalType.None;

            SaveManager.SaveGame();
        }
    }

    private void OpenModal()
    {
        _currentModal.GlobalPosition = SizeUtils.ScreenCenter + new Vector2(0, SizeUtils.ScreenH);
        _modalLayer.AddChild(_currentModal);
        TweenUtils.Travel(_currentModal, SizeUtils.ScreenCenter, 0.4f, Tween.TransitionType.Circ);
    }

    public enum ModalType
    {
        None,
        Victory,
        Settings,
    }
}