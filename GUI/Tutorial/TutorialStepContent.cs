using System.Collections.Generic;

public static class TutorialStepContent
{
    private static Dictionary<int, List<TutorialLocalManager.TutorialStep>> _tutorials = new()
    {
        { 1, new()
            {
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=15]PRESS A [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]MUNCHER[/color][/wave]\nTO SEE POSSIBLE MOVES[/font][/center]", new(1, 0)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, "[center][font gl=15]SWIPE TOWARDS [wave amp=40.0 freq=2.0 connected=1]FOOD[/wave]\nTO EAT IT![/font][/center]", new(1, 0), new(0, 0)),

                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=15]EAT ALL [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]COLORED FOOD[/color][/wave]\nTO REACH THE [wave amp=40.0 freq=2.0 connected=1]GOAL[/wave][/font][/center]", new(0, 0)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=15]EAT ALL [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]COLORED FOOD[/color][/wave]\nTO REACH THE [wave amp=40.0 freq=2.0 connected=1]GOAL[/wave][/font][/center]", new(0, 0), new(0, 1)),

                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=15]EAT ALL [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]GOALS[/color][/wave]\nTO WIN![/font][/center]", new(0, 1)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=15]EAT ALL [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]GOALS[/color][/wave]\nTO WIN![/font][/center]", new(0, 1), new(1, 1)),
            }
        },
        { 2, new()
            {
                // MOVE GREEN TWICE
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=15][wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]MUNCHERS[/color][/wave] CAN ONLY MOVE\nIN STRAIGHT LINES[/font][/center]", new(1, 2)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=15][wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]MUNCHERS[/color][/wave] CAN ONLY MOVE\nIN STRAIGHT LINES[/font][/center]", new(1, 2), new(2, 2)),
                
                new(TutorialLocalManager.TutorialStepType.StartMove, "[center][font gl=15]AS LONG AS NOTHING\nIS IN THE WAY[/font][/center]", new(2, 2)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, "[center][font gl=15]AS LONG AS NOTHING\nIS IN THE WAY[/font][/center]", new(2, 2), new(0, 2)),

                // CLEAR PATH WITH BLUE
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=15]CLEAR THE PATH WITH\nTHE [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]BLUE MUNCHER[/color][/wave]![/font][/center]", new(2, 0)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=15]CLEAR THE PATH WITH\nTHE [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]BLUE MUNCHER[/color][/wave]![/font][/center]", new(2, 0), new(1, 0)),
                
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=15]CLEAR THE PATH WITH\nTHE [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]BLUE MUNCHER[/color][/wave]![/font][/center]", new(1, 0)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=15]CLEAR THE PATH WITH\nTHE [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]BLUE MUNCHER[/color][/wave]![/font][/center]", new(1, 0), new(1, 1)),
                
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=15]CLEAR THE PATH WITH\nTHE [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]BLUE MUNCHER[/color][/wave]![/font][/center]", new(1, 1)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=15]CLEAR THE PATH WITH\nTHE [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]BLUE MUNCHER[/color][/wave]![/font][/center]", new(1, 1), new(0, 1)),
                
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=15]EAT ALL [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]GOALS[/color][/wave]\nTO WIN![/font][/center]", new(0, 1)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=15]EAT ALL [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]GOALS[/color][/wave]\nTO WIN![/font][/center]", new(0, 1), new(2, 1)),
                
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=15]EAT ALL [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]GOALS[/color][/wave]\nTO WIN![/font][/center]", new(0, 2)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=15]EAT ALL [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]GOALS[/color][/wave]\nTO WIN![/font][/center]", new(0, 2), new(0, 0)),
            }
        },
        { 3, new()
            {
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=15]ANY [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]MUNCHER[/color][/wave] CAN\nEAT [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Gray.GetColor().ToHtml()}]NEUTRAL FOOD[/color][/wave][/font][/center]", new(1, 2), new(1, 1)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=15]ANY [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]MUNCHER[/color][/wave] CAN\nEAT [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Gray.GetColor().ToHtml()}]NEUTRAL FOOD[/color][/wave][/font][/center]", new(2, 2), new(0, 2)),
                new(TutorialLocalManager.TutorialStepType.TextOnly, $"[center][font gl=15]EAT [wave amp=40.0 freq=2.0 connected=1]ALL[/wave] [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Gray.GetColor().ToHtml()}]NEUTRAL FOOD[/color][/wave]\nTO GET A [color=#{NamedColor.Yellow.GetColor().ToHtml()}]BONUS STAR![/color][/font][/center]"),
                new(TutorialLocalManager.TutorialStepType.TextOnly, $"[center][font gl=15]EAT [wave amp=40.0 freq=2.0 connected=1]ALL[/wave] [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Gray.GetColor().ToHtml()}]NEUTRAL FOOD[/color][/wave]\nTO GET A [color=#{NamedColor.Yellow.GetColor().ToHtml()}]BONUS STAR![/color][/font][/center]"),
                new(TutorialLocalManager.TutorialStepType.TextOnly, $"[center][font gl=15]FINISH A LEVEL WITH\n[wave amp=40.0 freq=2.0 connected=1]THE GIVEN UNDO COUNT[/wave]\nTO GET A [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Yellow.GetColor().ToHtml()}]BONUS STAR![/color][/wave][/font][/center]"),
                new(TutorialLocalManager.TutorialStepType.TextOnly, $"[center][font gl=15]FINISH A LEVEL WITH\n[wave amp=40.0 freq=2.0 connected=1]THE GIVEN UNDO COUNT[/wave]\nTO GET A [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Yellow.GetColor().ToHtml()}]BONUS STAR![/color][/wave][/font][/center]"),
                new(TutorialLocalManager.TutorialStepType.TextOnly, $"[center][font gl=15]FINISH A LEVEL WITH\n[wave amp=40.0 freq=2.0 connected=1]THE GIVEN UNDO COUNT[/wave]\nTO GET A [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Yellow.GetColor().ToHtml()}]BONUS STAR![/color][/wave][/font][/center]"),
            }
        }
    };

    public static List<TutorialLocalManager.TutorialStep> GetSteps(int levelId) => _tutorials.GetValueOrDefault(levelId);
    public static bool IsTutorial(int levelId) => _tutorials.ContainsKey(levelId);
}