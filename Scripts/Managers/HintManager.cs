using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class HintManager
{
    private static List<HintMove> _solutionPath = new();

    public static HintMove GetHint() => _solutionPath.FirstOrDefault();

    public static void CalculateSolutionPath()
    {
        _solutionPath = HintSystem.GetSolutionPath(LevelManager.Instance.Level);
    }

    public static void ActivateHint() => _solutionPath.RemoveAt(0);
}