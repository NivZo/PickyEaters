using Godot;

public partial class SettingsModal : Node2D
{
    private HSlider _musicSlider;
    private HSlider _soundEffectsSlider;
    public override void _Ready()
    {
        base._Ready();

        _musicSlider = GetNode<HSlider>("Modal/MusicVolume/MusicVolumeSlider");
        _musicSlider.Value = SaveManager.ActiveSave.MusicVolumeScale;
        _musicSlider.ValueChanged += OnMusicSlide;

        _soundEffectsSlider = GetNode<HSlider>("Modal/SoundEffectsVolume/SoundEffectsVolumeSlider");
        _soundEffectsSlider.Value = SaveManager.ActiveSave.SoundEffectsVolumeScale;
        _soundEffectsSlider.ValueChanged += OnSoundEffectsSlide;
    }

    private void OnMusicSlide(double value)
    {
        AudioManager.Instance.AdjustMusicVolume(value);
        GetNode<Sprite2D>("Modal/MusicVolume/IconMusicOn").Visible = value != 0;
        GetNode<Sprite2D>("Modal/MusicVolume/IconMusicOff").Visible = value == 0;
    }

    private void OnSoundEffectsSlide(double value)
    {
        AudioManager.Instance.AdjustSoundEffectsVolume(value);
        GetNode<Sprite2D>("Modal/SoundEffectsVolume/IconMusicOn").Visible = value != 0;
        GetNode<Sprite2D>("Modal/SoundEffectsVolume/IconMusicOff").Visible = value == 0;
    }
}
