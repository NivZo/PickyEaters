using System;
using Godot;

public static class CoinsManager
{
    public static int QuestReward = 200;

    public static void AddCoins(int amount)
    {
        AudioManager.PlaySoundEffect(AudioType.EarnCoins, 1.5f);
        SaveManager.ActiveSave.Coins += amount;
        SaveManager.CommitActiveSave();
    }
}