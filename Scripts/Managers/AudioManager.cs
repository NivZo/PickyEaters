using System;
using Godot;

public class AudioManager
{
    public static AudioManager Instance { get; } = new AudioManager();

    private AudioStreamPlayer _soundEffectPlayer;
    private AudioStreamPlayer _musicPlayer;

    public void Setup(AudioStreamPlayer soundEffectPlayer, AudioStreamPlayer musicPlayer)
    {
        _soundEffectPlayer = soundEffectPlayer;
        _musicPlayer = musicPlayer;
    }

    public void PlayAudio(AudioType audioType)
    {
        _soundEffectPlayer.Stream = GD.Load<AudioStream>($"res://Audio/{audioType}.wav");
        _soundEffectPlayer.Play();
    }

    public void AdjustMusicVolume(double volume)
    {
        _musicPlayer.VolumeDb = VolumeScaleToDB(volume);
        SaveManager.ActiveSave.MusicVolumeScale = volume;
    }

    public void AdjustSoundEffectsVolume(double volume)
    {
        _soundEffectPlayer.VolumeDb = VolumeScaleToDB(volume);
        SaveManager.ActiveSave.SoundEffectsVolumeScale = volume;
    }

    private float VolumeScaleToDB(double volume) => Mathf.LinearToDb((float)volume);
}