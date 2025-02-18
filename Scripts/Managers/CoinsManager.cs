using System;
using Godot;

public class CoinsManager
{
    public static CoinsManager Instance { get; } = new CoinsManager();

    public void AddCoins(int amount)
    {
        SaveManager.ActiveSave.Coins += amount;
        SaveManager.SaveGame();
    }
}