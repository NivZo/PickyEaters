using System;
using Godot;

public static class RandomUtils
{
    private static Random _rnd = new();

    public static float RandomInRange(float min, float max)
        => (float)_rnd.NextDouble() * (max - min) + min;
}