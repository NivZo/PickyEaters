using Godot;

public static class SizeUtils
{
    public static int ScreenH { get; } = 2560;
    public static int ScreenW { get; } = 1440;
    public static Vector2 ScreenCenter { get; } = new(ScreenW/2, ScreenH/2);
}