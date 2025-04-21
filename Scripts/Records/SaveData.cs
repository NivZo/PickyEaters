using System;
using Godot;

[GlobalClass]
public partial class SaveData : Resource
{
    [Export] public int LevelReached { get; set; } = 1;
    [Export] public int[] LevelStarsObtained { get; set;} = new int[LevelManager.MaxLevel + 1];
    [Export] public int Coins { get; set; } = 0;
    [Export] public int CurrentStars { get; set; } = 0;

    [Export] public long LastDailyFreeRewardClaimedUnixSec { get; set; } = 0;
    [Export] public long LastHourlyAdRewardClaimedUnixSec { get; set; } = 0;

    [Export] public double MusicVolumeScale { get; set; } = 1;
    [Export] public double SoundEffectsVolumeScale { get; set; } = 1;
    [Export] public double ScreenShakeStrength { get; set; } = 1;

    [Export] public Godot.Collections.Array<EaterFace> UnlockedFaces { get; set; } = new() { EaterFace.SmileBasic };
}