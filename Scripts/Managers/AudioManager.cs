using System;
using Godot;

public class AudioManager
{
    private static AudioStreamPlayer _soundEffectPlayer;
    private static AudioStreamPlayer _musicPlayer;

    public static void Setup(AudioStreamPlayer soundEffectPlayer, AudioStreamPlayer musicPlayer)
    {
        _soundEffectPlayer = soundEffectPlayer;
        _musicPlayer = musicPlayer;
    }

    public static void PlayAudio(AudioType audioType)
    {
        _soundEffectPlayer.Stream = GD.Load<AudioStream>($"res://Audio/{audioType}.wav");
        _soundEffectPlayer.Play();
    }

    public static void AdjustMusicVolume(double volume)
    {
        _musicPlayer.VolumeDb = VolumeScaleToDB(volume);
        SaveManager.ActiveSave.MusicVolumeScale = volume;
    }

    public static void AdjustSoundEffectsVolume(double volume)
    {
        _soundEffectPlayer.VolumeDb = VolumeScaleToDB(volume);
        SaveManager.ActiveSave.SoundEffectsVolumeScale = volume;
    }

    private static float VolumeScaleToDB(double volume) => Mathf.LinearToDb((float)volume);
}