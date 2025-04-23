using System;
using Godot;

public static class AudioManager
{
    private static AudioStreamPlayer _musicPlayer;
    private static AudioStreamPlayer _soundEffectPlayer;
    private static readonly float _soundEffectVolumeFactor = 0.8f;

    public static void Setup(AudioStreamPlayer soundEffectPlayer, AudioStreamPlayer musicPlayer)
    {
        _musicPlayer = musicPlayer;
        _soundEffectPlayer = soundEffectPlayer;
    }

    public static void PlaySoundEffect(AudioType audioType, float pitchFactor = 1)
    {
        var audio = GD.Load<AudioStream>($"res://Audio/{audioType}.wav");
        if (_soundEffectPlayer.Stream?.ResourcePath != audio.ResourcePath)
        {
            _soundEffectPlayer.Stream = audio;
        }

        _soundEffectPlayer.PitchScale = pitchFactor * RandomUtils.RandomInRange(0.95f, 1.05f);
        _soundEffectPlayer.Play();
    }

    public static void PlayTitleBackgroundMusic()
    {
        _musicPlayer.VolumeDb = VolumeScaleToDB(SaveManager.ActiveSave.MusicVolumeScale * .25f);
        _musicPlayer.Play();
    }

    public static void PlayBackgroundMusic()
    {
        TweenUtils.MethodTween(_musicPlayer, val => _musicPlayer.VolumeDb = val.As<float>(), VolumeScaleToDB(SaveManager.ActiveSave.MusicVolumeScale * .25f), VolumeScaleToDB(SaveManager.ActiveSave.MusicVolumeScale), 1f);
    }

    public static void AdjustMusicVolume(double volume)
    {
        _musicPlayer.VolumeDb = VolumeScaleToDB(volume);
        SaveManager.ActiveSave.MusicVolumeScale = volume;
    }

    public static void AdjustSoundEffectsVolume(double volume)
    {
        _soundEffectPlayer.VolumeDb = VolumeScaleToDB(volume * _soundEffectVolumeFactor);
        SaveManager.ActiveSave.SoundEffectsVolumeScale = volume;
    }

    private static float VolumeScaleToDB(double volume) => Mathf.LinearToDb((float)volume);
}