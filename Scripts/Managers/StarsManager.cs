using System;
using Godot;

public static class StarsManager
{
    public static int RequiredStars = 10;
    public static void AddStar(int levelId)
    {
        var currStars = SaveManager.ActiveSave.LevelStarsObtained[levelId];
        SaveManager.ActiveSave.LevelStarsObtained[levelId] = Math.Max(currStars + 1, currStars);
        SaveManager.ActiveSave.CurrentStars++;
        EventManager.InvokeStarIncrease();

        if (SaveManager.ActiveSave.CurrentStars >= RequiredStars)
        {
            SaveManager.ActiveSave.CurrentStars %= RequiredStars;
            EventManager.InvokeStarsComplete();
        }
    }
}