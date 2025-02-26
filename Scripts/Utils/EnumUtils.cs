
using System;
using System.Linq;
using System.Collections.Generic;

public static class EnumUtils
{
    static Random _rnd = new Random();

    public static T GetRandomValue<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(_rnd.Next(values.Length));
    }

    public static T GetRandomValueOutOf<T>(IEnumerable<T> values) where T : Enum
    {
        return values.ToArray()[_rnd.Next(values.Count())];
    }

    public static T GetRandomValueExcluding<T>(IEnumerable<T> excluded) where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).OfType<T>();
        var afterExclusion = values.Where(value => !excluded.Contains(value)).ToList();
        return afterExclusion[_rnd.Next(afterExclusion.Count)];
    }
}