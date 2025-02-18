using Godot;

public static class TextUtils
{
    public static string WaveString(string text, float frequency = 4, float amplitude = 32, bool center = true)
    {
        var result = $"[wave amp={amplitude} freq={frequency} connected=0][font gl=5]{text}[/font][/wave]";
        if (center)
        {
            result = AddAttribute(result, "center");
        }

        return result;
    }

    public static string AddAttribute(string text, string attribute) => $"[{attribute}]{text}[/{attribute}]";
}