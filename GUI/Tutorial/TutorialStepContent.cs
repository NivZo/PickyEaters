using System.Collections.Generic;

public static class TutorialStepContent
{
    private static Dictionary<int, List<TutorialLocalManager.TutorialStep>> _tutorials = new()
    {
        { 1, new()
            {
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=10]PRESS AND HOLD A [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]MUNCHER[/color][/wave]\nTO SEE POSSIBLE MOVES[/font][/center]", new(1, 0)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=10]SWIPE TOWARDS [color=#{NamedColor.Green.GetColor().ToHtml()}][wave amp=40.0 freq=2.0 connected=1]FOOD[/wave][/color]\nTO EAT IT![/font][/center]", new(1, 0), new(0, 0)),

                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=10]MUNCHERS MUST EAT ALL\n[wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]FOOD OF THEIR COLOR[/color][/wave][/font][/center]", new(0, 0)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=10]MUNCHERS MUST EAT ALL\n[wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]FOOD OF THEIR COLOR[/color][/wave][/font][/center]", new(0, 0), new(0, 1)),

                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=10]EAT THE [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]STAR FOOD[/color][/wave]\nTO WIN![/font][/center]", new(0, 1)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=10]EAT THE [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]STAR FOOD[/color][/wave]\nTO WIN![/font][/center]", new(0, 1), new(1, 1)),
            }
        },
        { 2, new()
            {
                // MOVE GREEN TWICE
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=10][wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]MUNCHERS[/color][/wave] CAN ONLY MOVE\nIN STRAIGHT LINES[/font][/center]", new(1, 2)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=10][wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]MUNCHERS[/color][/wave] CAN ONLY MOVE\nIN STRAIGHT LINES[/font][/center]", new(1, 2), new(2, 2)),
                
                new(TutorialLocalManager.TutorialStepType.StartMove, "[center][font gl=10]AS LONG AS NOTHING\nIS IN THE WAY[/font][/center]", new(2, 2)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, "[center][font gl=10]AS LONG AS NOTHING\nIS IN THE WAY[/font][/center]", new(2, 2), new(0, 2)),

                // CLEAR PATH WITH BLUE
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=10]CLEAR THE PATH WITH\nTHE [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]BLUE MUNCHER[/color][/wave]![/font][/center]", new(2, 0)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=10]CLEAR THE PATH WITH\nTHE [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]BLUE MUNCHER[/color][/wave]![/font][/center]", new(2, 0), new(1, 0)),
                
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=10]CLEAR THE PATH WITH\nTHE [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]BLUE MUNCHER[/color][/wave]![/font][/center]", new(1, 0)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=10]CLEAR THE PATH WITH\nTHE [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]BLUE MUNCHER[/color][/wave]![/font][/center]", new(1, 0), new(1, 1)),
                
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=10][wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]STAR FOOD[/color][/wave] MUST BE\nEATEN LAST![/font][/center]", new(1, 1)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=10][wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]STAR FOOD[/color][/wave] MUST BE\nEATEN LAST![/font][/center]", new(1, 1), new(0, 1)),
                
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=10]EAT ALL [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]STAR FOOD[/color][/wave]\nTO WIN![/font][/center]", new(0, 1)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=10]EAT ALL [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]STAR FOOD[/color][/wave]\nTO WIN![/font][/center]", new(0, 1), new(2, 1)),
                
                new(TutorialLocalManager.TutorialStepType.StartMove, $"[center][font gl=10]EAT ALL [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]STAR FOOD[/color][/wave]\nTO WIN![/font][/center]", new(0, 2)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=10]EAT ALL [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]STAR FOOD[/color][/wave]\nTO WIN![/font][/center]", new(0, 2), new(0, 0)),
            }
        },
        { 3, new()
            {
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=10]ANY [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Blue.GetColor().ToHtml()}]MUNCHER[/color][/wave] CAN\nEAT [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Gray.GetColor().ToHtml()}]NEUTRAL FOOD[/color][/wave][/font][/center]", new(1, 2), new(1, 1)),
                new(TutorialLocalManager.TutorialStepType.PerformMove, $"[center][font gl=10]ANY [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Green.GetColor().ToHtml()}]MUNCHER[/color][/wave] CAN\nEAT [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Gray.GetColor().ToHtml()}]NEUTRAL FOOD[/color][/wave][/font][/center]", new(2, 2), new(0, 2)),
                new(TutorialLocalManager.TutorialStepType.TextOnly, $"[center][font gl=10]EAT [color=#{NamedColor.Yellow.GetColor().ToHtml()}][wave amp=40.0 freq=2.0 connected=1]50%[/wave][/color] OF [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Gray.GetColor().ToHtml()}]NEUTRAL FOOD[/color][/wave]\nTO GET [color=#{NamedColor.Yellow.GetColor().ToHtml()}]2 STARS![/color][/font][/center]"),
                new(TutorialLocalManager.TutorialStepType.TextOnly, $"[center][font gl=10]EAT [color=#{NamedColor.Yellow.GetColor().ToHtml()}][wave amp=40.0 freq=2.0 connected=1]50%[/wave][/color] OF [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Gray.GetColor().ToHtml()}]NEUTRAL FOOD[/color][/wave]\nTO GET [color=#{NamedColor.Yellow.GetColor().ToHtml()}]2 STARS![/color][/font][/center]"),
                new(TutorialLocalManager.TutorialStepType.TextOnly, $"[center][font gl=10]EAT [color=#{NamedColor.Yellow.GetColor().ToHtml()}][wave amp=40.0 freq=2.0 connected=1]100%[/wave][/color] OF [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Gray.GetColor().ToHtml()}]NEUTRAL FOOD[/color][/wave]\nTO GET [color=#{NamedColor.Yellow.GetColor().ToHtml()}]3 STARS![/color][/font][/center]"),
                new(TutorialLocalManager.TutorialStepType.TextOnly, $"[center][font gl=10]EAT [color=#{NamedColor.Yellow.GetColor().ToHtml()}][wave amp=40.0 freq=2.0 connected=1]100%[/wave][/color] OF [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Gray.GetColor().ToHtml()}]NEUTRAL FOOD[/color][/wave]\nTO GET [color=#{NamedColor.Yellow.GetColor().ToHtml()}]3 STARS![/color][/font][/center]"),
                new(TutorialLocalManager.TutorialStepType.TextOnly, $"[center][font gl=10]EAT [color=#{NamedColor.Yellow.GetColor().ToHtml()}][wave amp=40.0 freq=2.0 connected=1]100%[/wave][/color] OF [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Gray.GetColor().ToHtml()}]NEUTRAL FOOD[/color][/wave]\nTO GET [color=#{NamedColor.Yellow.GetColor().ToHtml()}]3 STARS![/color][/font][/center]"),
                // new(TutorialLocalManager.TutorialStepType.TextOnly, $"[center][font gl=10]FINISH WITH [wave amp=40.0 freq=2.0 connected=1]LESS UNDOS[/wave]\nTO GET A [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Yellow.GetColor().ToHtml()}]BONUS STAR![/color][/wave][/font][/center]"),
                // new(TutorialLocalManager.TutorialStepType.TextOnly, $"[center][font gl=10]FINISH WITH [wave amp=40.0 freq=2.0 connected=1]LESS UNDOS[/wave]\nTO GET A [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Yellow.GetColor().ToHtml()}]BONUS STAR![/color][/wave][/font][/center]"),
                // new(TutorialLocalManager.TutorialStepType.TextOnly, $"[center][font gl=10]FINISH WITH [wave amp=40.0 freq=2.0 connected=1]LESS UNDOS[/wave]\nTO GET A [wave amp=40.0 freq=2.0 connected=1][color=#{NamedColor.Yellow.GetColor().ToHtml()}]BONUS STAR![/color][/wave][/font][/center]"),
            }
        }
    };

    public static List<TutorialLocalManager.TutorialStep> GetSteps(int levelId) => _tutorials.GetValueOrDefault(levelId);
    public static bool IsTutorial(int levelId) => _tutorials.ContainsKey(levelId);
}