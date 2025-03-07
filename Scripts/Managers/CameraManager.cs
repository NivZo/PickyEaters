using System;
using Godot;

public static class CameraManager
{
    private static Camera2D _camera;

    public static void Setup(Camera2D camera2D)
    {
        _camera = camera2D;
    }
}