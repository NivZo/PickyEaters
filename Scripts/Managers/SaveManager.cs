using System;
using System.Linq;
using Godot;

public class SaveManager
{
    private const string USER_SAVEFILE = "user://savegame.tres";
    private const string LOCAL_SAVEFILE = "res://savegame.tres";

    private static string SAVEFILE => SaveLocally ? LOCAL_SAVEFILE : USER_SAVEFILE;

    public static SaveData ActiveSave = new();
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
        Main.ErrorDisplay = "Load1";
        ActiveSave = GetCurrentSave();
        Main.ErrorDisplay = "Load2";
        AudioManager.AdjustMusicVolume(ActiveSave.MusicVolumeScale);
        Main.ErrorDisplay = "Load3";
        AudioManager.AdjustSoundEffectsVolume(ActiveSave.SoundEffectsVolumeScale);
        Main.ErrorDisplay = "Load4";
        ScreenManager.LoadFirstScreen();
        Main.ErrorDisplay = "Load5";
    }

    public static void EraseSave()
    {
        ActiveSave = new SaveData() {
                LevelReached = 1,
            };
        ResourceSaver.Save(ActiveSave, SAVEFILE);
        ScreenManager.TransitionToScreen(ScreenManager.ScreenType.MainMenu);
    }

    public static void OverrideDevSave()
    {
        ActiveSave = new SaveData()
        {
            LevelReached = 101,
            Coins = 2475,
            UnlockedFaces = new() { EaterFace.SmileBasic, EaterFace.Vampire, EaterFace.CatEyes, EaterFace.CatEyes, EaterFace.CuteFang, EaterFace.WideOpenSmile },
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
                GD.Print("Loaded ", currSave.LevelReached);
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