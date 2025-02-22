
using System;
using System.Collections.Generic;

public static class EnumUtils
{
    static Random _rnd = new Random();

    public static T GetRandomValue<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(_rnd.Next(values.Length));
    }

    public static T GetRandomValueOutOf<T>(List<T> values) where T : Enum
    {
        return values[_rnd.Next(values.Count)];
    }
}