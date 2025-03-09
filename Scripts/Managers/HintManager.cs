using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class HintManager
{
    public static bool IsHintAvailable() => _movesSinceLastHint == 0;
    public static bool IsOutOfHints() => HintsLeft == 0 && _movesSinceLastHint == 0;
    public static int HintsPerClick { get; private set; } = 0;
    public static int HintsLeft { get; private set; } = 2;
    private static int _movesSinceLastHint = 0;
    private static HintMove[] _solutionPath;
    private static int _currHint = 0;

    public static HintMove GetHint() => _solutionPath[_currHint];

    public static void HintUsed() => HintsLeft -= 1;
    public static void ResetHintUsed() => HintsLeft = 2;

    public static void CalculateSolutionPath()
    {
        _solutionPath = HintSystem.GetHints(LevelManager.Level);
        HintsPerClick = Math.Max(3, LevelManager.Level.GetFood().Count / 10 + 1);
        GD.Print("Hints per click: ", HintsPerClick);
        _currHint = 0;
        _movesSinceLastHint = 0;
        ResetHintUsed();
    }

    public static void ActivateHint()
    {
        _currHint++;
    }

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