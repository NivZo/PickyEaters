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

    public static void PlayAudio(AudioType audioType, float pitchFactor = 1)
    {
        var audio = GD.Load<AudioStream>($"res://Audio/{audioType}.wav");
        if (_soundEffectPlayer.Stream?.ResourcePath != audio.ResourcePath)
        {
            _soundEffectPlayer.Stream = audio;
        }

        _soundEffectPlayer.PitchScale = pitchFactor * RandomUtils.RandomInRange(0.9f, 1.1f);
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