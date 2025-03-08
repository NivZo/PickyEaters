using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class HintManager
{
    public static bool IsHintAvailable() => _movesSinceLastHint == 0;
    private static int _movesSinceLastHint = 0;
    private static HintMove[] _solutionPath;
    private static int _currHint = 0;

    public static HintMove GetHint() => _solutionPath[_currHint];

    public static void CalculateSolutionPath()
    {
        _solutionPath = HintSystem.GetHints(LevelManager.Level);
        _currHint = 0;
        _movesSinceLastHint = 0;
    }

    public static void ActivateHint() => _currHint++;
    public static void DeactivateHint() => _currHint--;
    public static void HandleNonHintMove() => _movesSinceLastHint++;
    public static void HandleUndo()
    {
        if (_movesSinceLastHint == 0)
        {
            _currHint--;
        }
        else
        {
            _movesSinceLastHint--;
        }
    }
}