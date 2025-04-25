using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class LevelManager
{
    private static readonly Dictionary<int, Color> _levelColors = new()
    {
        { 1, TierColor.Easy.GetColor() },
        { 16, TierColor.Medium.GetColor() },
        { 46, TierColor.Hard.GetColor() },
        { 106, TierColor.Expert.GetColor() },
        { 181, TierColor.Genius.GetColor() },
        { 271, TierColor.Master.GetColor() },
    };

    private static CanvasLayer _gameLayer;
    private static Level _level;
    private static Lazy<int> _maxLevelLazy = new(DirAccess.GetFilesAt("res://AssetGeneration/Levels/").Length);
    private static int _totalWhiteFoodCount = 0;

    public static int CurrentLevelId;
    public static Level Level => _level;
    public static int MaxLevel { get => _maxLevelLazy.Value; }

    public static void Setup(CanvasLayer gameLayer)
    {
        _gameLayer = gameLayer;
        CurrentLevelId = CurrentLevelId == default ? SaveManager.ActiveSave.LevelReached : CurrentLevelId;
    }

    public static bool IsVictory() => _level.Food.GetChildren().Where(child => child is Food food && food.FoodType != FoodType.White).Count() == 0;
    public static bool IsTwoStarVictory() => _level.Food.GetChildren().Where(child => child is Food food && food.FoodType == FoodType.White).Count() <= _totalWhiteFoodCount / 2;
    public static bool IsThreeStarVictory() => _level.Food.GetChildren().Where(child => child is Food food && food.FoodType == FoodType.White).Count() == 0;
    public static bool CanEatLast(FoodType foodType) => _level.Food.GetChildren().Where(child => child is Food food && food.FoodType == foodType).Count() == 1;

    public static void LoadLevel(int levelId)
    {
        if (_gameLayer.GetChildren().Count == 0) { _level = null; }
        if (_level != null)
        {
            _gameLayer.RemoveChild(_level);
            _level.QueueFree();
        }
        
        CurrentLevelId = Math.Min(MaxLevel, levelId);
        BackgroundManager.ChangeColor(GetLevelColor(CurrentLevelId), lightenFactor: 0.4f);
        HistoryManager.ResetHistory();

        _level = GD.Load<PackedScene>($"res://AssetGeneration/Levels/Level{CurrentLevelId}.tscn").Instantiate<Level>();
        _gameLayer.AddChild(_level);

        _gameLayer.GetNode<RichTextLabel>("%LevelTitle").Text = TextUtils.WaveString($"LEVEL {levelId}");

        if (TutorialStepContent.IsTutorial(CurrentLevelId))
        {
            var tut = TutorialLocalManager.Create(TutorialStepContent.GetSteps(CurrentLevelId));
            _level.AddChild(tut);
        }

        _totalWhiteFoodCount = _level.Food.GetChildren().Where(child => child is Food food && food.FoodType == FoodType.White).Count();
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

    public static Color GetLevelColor(int levelId)
    {
        var color = _levelColors.Keys.Where(key => key <= levelId).Max();
        return _levelColors[color];
    }
}