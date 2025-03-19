using System;
using Godot;

public class AudioManager
{
    private static AudioStreamPlayer _musicPlayer;
    private static AudioStreamPlayer _soundEffectPlayer;

    public static void Setup(AudioStreamPlayer soundEffectPlayer, AudioStreamPlayer musicPlayer)
    {
        _musicPlayer = musicPlayer;
        _soundEffectPlayer = soundEffectPlayer;
    }

    public static void PlayAudio(AudioType audioType, float pitchFactor = 1)
    {
        var audio = GD.Load<AudioStream>($"res://Audio/{audioType}.wav");
        if (_soundEffectPlayer.Stream?.ResourcePath != audio.ResourcePath)
        {
            _soundEffectPlayer.Stream = audio;
        }

        _soundEffectPlayer.PitchScale = pitchFactor * RandomUtils.RandomInRange(0.95f, 1.05f);
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