using System.Linq;
using Godot;

public static class HintSystem
{
    public static HintMove[] GetHints(Level level)
    {
        var packedSolution = level.GetMeta("solution").As<Godot.Collections.Array<Vector2I>>().ToArray();
        return GetSolution(packedSolution);
    }

    private static HintMove[] GetSolution(Vector2I[] packedSolution)
    {
        var solution = new HintMove[packedSolution.Length/2];
        var i = solution.Length-1;
        foreach (var chunk in packedSolution.Chunk(2))
        {
            solution[i] = new HintMove()
            {
                From = new Vector2I(chunk[0].X, chunk[0].Y),
                To = new Vector2I(chunk[1].X, chunk[1].Y),
            };
            i--;
        }

        return solution;
    }
}