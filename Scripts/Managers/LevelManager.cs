using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class LevelManager
{
    private static CanvasLayer _gameLayer;
    private static Level _level;
    private static Lazy<int> _maxLevelLazy = new(DirAccess.GetFilesAt("res://Levels/").Length-1);

    public static int CurrentLevelId;
    public static Level Level => _level;
    public static int MaxLevel { get => _maxLevelLazy.Value; }

    public static void Setup(CanvasLayer gameLayer)
    {
        _gameLayer = gameLayer;
        CurrentLevelId = CurrentLevelId == default ? SaveManager.ActiveSave.LevelReached : CurrentLevelId;
    }

    public static bool IsVictory() => _level.Food.GetChildren().Where(child => child is Food food && food.FoodType != FoodType.White).Count() == 0;
    public static bool IsFlawlessVictory() => _level.Food.GetChildren().Where(child => child is Food food && food.FoodType == FoodType.White).Count() == 0;
    public static bool CanEatLast(FoodType foodType) => _level.Food.GetChildren().Where(child => child is Food food && food.FoodType == foodType).Count() == 1;

    public static void LoadLevel(int levelId)
    {
        if (_gameLayer.GetChildren().Count == 0) { _level = null; }
        if (_level != null)
        {
            _gameLayer.RemoveChild(_level);
            _level.QueueFree();
        }
        
        HistoryManager.ResetHistory();

        CurrentLevelId = Math.Min(MaxLevel, levelId);
        _level = GD.Load<PackedScene>($"res://Levels/Level{CurrentLevelId}.tscn").Instantiate<Level>();
        _gameLayer.AddChild(_level);

        _gameLayer.GetNode<RichTextLabel>("%LevelTitle").Text = TextUtils.WaveString($"LEVEL {levelId}");
    }

    public static void ResetLevel() => LoadLevel(CurrentLevelId);

    public static void IncreaseLevelReached()
    {
        if (CurrentLevelId == SaveManager.ActiveSave.LevelReached && CurrentLevelId < MaxLevel)
        {
            SaveManager.ActiveSave.LevelReached += 1;
            SaveManager.CommitActiveSave();
        }
    }

    public static void NextLevel()
    {
        LoadLevel(CurrentLevelId+1);
    }

    public static void NextTenLevels()
    {
        LoadLevel(CurrentLevelId+10);
    }


    public static void PreviousLevel()
    {
        LoadLevel(CurrentLevelId-1);
    }
}