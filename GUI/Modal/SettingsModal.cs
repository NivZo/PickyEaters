using Godot;

public partial class SettingsModal : Node2D
{
    private HSlider _musicSlider;
    private HSlider _soundEffectsSlider;
    private HSlider _screenShake;
    public override void _Ready()
    {
        base._Ready();

        _musicSlider = GetNode<HSlider>("Modal/MusicVolume/MusicVolumeSlider");
        _musicSlider.Value = SaveManager.ActiveSave.MusicVolumeScale;
        _musicSlider.ValueChanged += OnMusicSlide;
        OnMusicSlide(_musicSlider.Value);

        _soundEffectsSlider = GetNode<HSlider>("Modal/SoundEffectsVolume/SoundEffectsVolumeSlider");
        _soundEffectsSlider.Value = SaveManager.ActiveSave.SoundEffectsVolumeScale;
        _soundEffectsSlider.ValueChanged += OnSoundEffectsSlide;
        OnSoundEffectsSlide(_soundEffectsSlider.Value);

        _screenShake = GetNode<HSlider>("Modal/ScreenShake/ScreenShakeStrengthSlider");
        _screenShake.Value = SaveManager.ActiveSave.ScreenShakeStrength;
        _screenShake.ValueChanged += OnScreenShakeSlide;
        OnScreenShakeSlide(_screenShake.Value);
    }

    private void OnMusicSlide(double value)
    {
        AudioManager.AdjustMusicVolume(value);
        GetNode<Sprite2D>("Modal/MusicVolume/IconMusicOn").Visible = value != 0;
        GetNode<Sprite2D>("Modal/MusicVolume/IconMusicOff").Visible = value == 0;
    }

    private void OnSoundEffectsSlide(double value)
    {
        AudioManager.AdjustSoundEffectsVolume(value);
        GetNode<Sprite2D>("Modal/SoundEffectsVolume/IconMusicOn").Visible = value != 0;
        GetNode<Sprite2D>("Modal/SoundEffectsVolume/IconMusicOff").Visible = value == 0;
    }

    private void OnScreenShakeSlide(double value)
    {
        SaveManager.ActiveSave.ScreenShakeStrength = value;
        GetNode<RichTextLabel>("Modal/ScreenShake/ScreenShakeValueLabel").Text = $"{(int)(value * 100)}";
    }
}
