using System;
using System.Linq;
using Godot;

public class LevelManager
{
    public static LevelManager Instance { get; } = new LevelManager();
    
    private CanvasLayer _gameLayer;
    private Level _level;

    private int _levelId = 1;
    public int CurrentLevelId => _levelId;


    public Level Level => _level;
    public int MaxLevel = 130;

    public void Setup(CanvasLayer gameLayer)
    {
        _gameLayer = gameLayer;
    }

    public bool IsVictory() => _level.Food.GetChildren().Where(child => child is Food food && food.FoodType != FoodType.White).Count() == 0;
    public bool CanEatLast(FoodType foodType) => _level.Food.GetChildren().Where(child => child is Food food && food.FoodType == foodType).Count() == 1;

    public void LoadLevel(int levelId)
    {
        if (_gameLayer.GetChildren().Count == 0) { _level = null; }
        if (_level != null)
        {
            _gameLayer.RemoveChild(_level);
            _level.QueueFree();
        }
        
        HistoryManager.Instance.ResetHistory();

        _levelId = Math.Min(MaxLevel, levelId);
        _level = GD.Load<PackedScene>($"res://Levels/Level{_levelId}.tscn").Instantiate<Level>();
        _gameLayer.AddChild(_level);

        _gameLayer.GetNode<RichTextLabel>("%LevelTitle").Text = TextUtils.WaveString($"LEVEL {levelId}");
    }

    public void ResetLevel() => LoadLevel(_levelId);

    public void IncreaseLevelReached()
    {
        if (_levelId == SaveManager.ActiveSave.LevelReached)
        {
            SaveManager.ActiveSave.LevelReached += 1;
            SaveManager.SaveGame();
        }
    }

    public void NextLevel()
    {
        LoadLevel(_levelId+1);
    }

    public void NextTenLevels()
    {
        LoadLevel(_levelId+10);
    }


    public void PreviousLevel()
    {
        LoadLevel(_levelId-1);
    }
}