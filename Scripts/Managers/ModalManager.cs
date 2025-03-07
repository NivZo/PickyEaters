using System;
using System.Linq;
using Godot;

public class ModalManager
{
    private static ModalType[] UNCLOSABLE_MODALS = new ModalType[] { ModalType.Victory };

    private static Node2D _currentModal = null;
    private static CanvasLayer _modalLayer;

    public static ModalType CurrentOpenModal = ModalType.None;

    public static void Setup(CanvasLayer modalLayer)
    {
        _modalLayer = modalLayer;
    }
    
    public static void OpenVictoryModal()
    {
        LevelManager.Instance.IncreaseLevelReached();

        _currentModal = GD.Load<PackedScene>("res://GUI/Modal/VictoryModal.tscn").Instantiate<Node2D>();
        CurrentOpenModal = ModalType.Victory;

        OpenModal();
    }

    public static void OpenSettingsModal()
    {
        _currentModal = GD.Load<PackedScene>("res://GUI/Modal/SettingsModal.tscn").Instantiate<Node2D>();
        CurrentOpenModal = ModalType.Settings;

        OpenModal();
    }

    public static void CloseModal(bool overideUnclosable = false)
    {
        if (_currentModal != null && (overideUnclosable || !UNCLOSABLE_MODALS.Contains(CurrentOpenModal)))
        {
            _currentModal.QueueFree();
            _currentModal = null;
            CurrentOpenModal = ModalType.None;

            SaveManager.SaveGame();
        }
    }

    private static void OpenModal()
    {
        _currentModal.GlobalPosition = SizeUtils.ScreenCenter + new Vector2(0, SizeUtils.ScreenH);
        _modalLayer.AddChild(_currentModal);
        TweenUtils.Travel(_currentModal, SizeUtils.ScreenCenter, 0.4f, Tween.TransitionType.Back);
    }

    public enum ModalType
    {
        None,
        Victory,
        Settings,
    }
}