using System;
using Godot;

public static class CoinsManager
{
    public static void AddCoins(int amount)
    {
        SaveManager.ActiveSave.Coins += amount;
        SaveManager.CommitActiveSave();
    }
}