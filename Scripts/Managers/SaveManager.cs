using System;
using System.Linq;
using Godot;

public class SaveManager
{
    private const string USER_SAVEFILE = "user://savegame.tres";
    private const string LOCAL_SAVEFILE = "res://savegame.tres";

    private static string SAVEFILE => SaveLocally ? LOCAL_SAVEFILE : USER_SAVEFILE;
    
    private static Lazy<SaveData> _activeSaveLazy = new(() => GetCurrentSave());

    public static SaveData ActiveSave {
        get => _activeSaveLazy.Value;
        set => _activeSaveLazy = new(() => value);
    }

    public static bool SaveLocally = false;

    public static void SaveGame()
    {
        var currSave = GetCurrentSave();
        ActiveSave.LevelReached = new int[3] { currSave.LevelReached, LevelManager.Instance.CurrentLevelId, ActiveSave.LevelReached }.Max();
        GD.Print("Saving ", ActiveSave.LevelReached);
        ResourceSaver.Save(ActiveSave, SAVEFILE);
    }

    public static void LoadGame()
    {
        ActiveSave = GetCurrentSave();

        AudioManager.Instance.AdjustMusicVolume(ActiveSave.MusicVolumeScale);
        AudioManager.Instance.AdjustSoundEffectsVolume(ActiveSave.SoundEffectsVolumeScale);
    }

    public static void EraseSave()
    {
        ResourceSaver.Save(new SaveData() {
                LevelReached = 1,
            }, SAVEFILE);
        LoadGame();
    }

    private static SaveData GetCurrentSave()
    {
        if (ResourceLoader.Exists(SAVEFILE))
        {
            var currSave = ResourceLoader.Load<SaveData>(SAVEFILE, null, ResourceLoader.CacheMode.Ignore);
            GD.Print("Loaded ", currSave.LevelReached);
            return currSave;
        }
        else
        {
            return new() {
                LevelReached = 1,
            };
        }
    }
}