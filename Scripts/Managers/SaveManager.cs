using System;
using System.Linq;
using Godot;

public class SaveManager
{
    private const string USER_SAVEFILE = "user://savegame.tres";
    private const string LOCAL_SAVEFILE = "res://savegame.tres";

    private static string SAVEFILE => SaveLocally ? LOCAL_SAVEFILE : USER_SAVEFILE;

    public static SaveData ActiveSave { 
        get => _activeSave;
        set {
            _activeSave = value;
            CommitActiveSave();
        }}
    private static SaveData _activeSave = new();
    public static bool SaveLocally = false;

    public static void CommitActiveSave()
    {
        GD.Print("Committing Active Save");
        ResourceSaver.Save(ActiveSave, SAVEFILE);
        EventManager.InvokeActiveSaveChange();
    }

    public static void LoadGame()
    {
        ActiveSave = GetCurrentSave();
        AudioManager.AdjustMusicVolume(ActiveSave.MusicVolumeScale);
        AudioManager.AdjustSoundEffectsVolume(ActiveSave.SoundEffectsVolumeScale);
        ScreenManager.LoadFirstScreen();
    }

    public static void EraseSave()
    {
        ActiveSave = new();
        ResourceSaver.Save(ActiveSave, SAVEFILE);
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.MainMenu);
    }

    public static void OverrideDevSave()
    {
        ActiveSave = new SaveData()
        {
            LevelReached = LevelManager.MaxLevel,
            Coins = 7425,
            UnlockedFaces = new() { EaterFace.SmileBasic, EaterFace.WideOpenSmile },
        };
        ResourceSaver.Save(ActiveSave, SAVEFILE);
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.MainMenu);
    }

    private static SaveData GetCurrentSave()
    {
        try
        {
            if (ResourceLoader.Exists(SAVEFILE))
            {
                var currSave = ResourceLoader.Load<SaveData>(SAVEFILE, null, ResourceLoader.CacheMode.Ignore);
                return currSave;
            }
            else
            {
                return new();
            }
        }
        catch
        {
            return new();
        }
    }
}