using Godot;
using System;

public partial class MainCamera : Camera2D
{
    [Export] public float RandomStrength = 30f;
    [Export] public float ShakeFade = 5f;

    private static float _shakeStrength = 0;
    private static MainCamera _camera;

    public override void _Ready()
    {
        base._Ready();
        _camera = this;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_shakeStrength != 0)
        {
            _shakeStrength = Mathf.Max(0, Mathf.Lerp(_shakeStrength, 0, _camera.ShakeFade * (float)delta));
            Offset = RandomOffset();

            if (_shakeStrength < 0.1 && _shakeStrength > -0.1)
            {
                _shakeStrength = 0;
            }
        }
    }

    public static void ApplyShake()
    {
        _shakeStrength = _camera.RandomStrength * (float)SaveManager.ActiveSave.ScreenShakeStrength;
        Input.VibrateHandheld(100, (float)SaveManager.ActiveSave.ScreenShakeStrength * 0.2f);
    }

    private Vector2 RandomOffset()
        => new(RandomUtils.RandomInRange(-_shakeStrength, _shakeStrength), RandomUtils.RandomInRange(-_shakeStrength, _shakeStrength));
}
