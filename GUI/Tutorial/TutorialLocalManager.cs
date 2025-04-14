using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class TutorialLocalManager : Node
{
    private int _currStepIdx = 0;
    private List<TutorialStep> _steps = new();
    private TutorialStep _currStep => _steps.ElementAtOrDefault(_currStepIdx);
    private TutorialStep _prevStep => _steps.ElementAtOrDefault(_currStepIdx-1);
    private HandGuidanceIndicator _ind;
    private RichTextLabel _tutorialText;
    private int _suggestUndo = 0;

    public override void _Ready()
    {
        EventManager.MoveSelectionStarted += HandleSelectionStarted;
        EventManager.MoveSelectionCancelled += HandleSelectionCancelled;
        EventManager.MovePerformed += HandleMovePerformed;
        EventManager.MoveUndone += HandleMoveUndone;

        _tutorialText = GetNode<RichTextLabel>("TutorialText");

        SetupStep();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventManager.MoveSelectionStarted -= HandleSelectionStarted;
        EventManager.MoveSelectionCancelled -= HandleSelectionCancelled;
        EventManager.MovePerformed -= HandleMovePerformed;
        EventManager.MoveUndone -= HandleMoveUndone;
    }

    public enum TutorialStepType
    {
        TextOnly,
        StartMove,
        PerformMove,
    }

    public record TutorialStep(TutorialStepType Type, string Text, Vector2? Position = null, Vector2? TargetPosition = null);

    public static TutorialLocalManager Create(List<TutorialStep> steps)
    {
        var manager = GD.Load<PackedScene>("res://GUI/Tutorial/TutorialLocalManager.tscn").Instantiate<TutorialLocalManager>();
        manager._steps = steps;
        manager._currStepIdx = 0;

        return manager;
    }

    private void SetupStep()
    {
        if (_ind != null)
        {
            _ind.QueueFree();
            _ind = null;
        }

        if (_suggestUndo > 0)
        {
            _tutorialText.Text = $"[center][font gl=15]PRESS THE [color=#{NamedColor.Yellow.GetColor().ToHtml()}]UNDO[/color] BUTTON\nTO GO BACK A STEP[/font][/center]";
            _tutorialText.Scale = new(.7f, .7f);
            TweenUtils.Pop(_tutorialText, 1, .5f);
        }
        
        else if (_currStep != null)
        {
            if (_currStep.Type == TutorialStepType.StartMove)
            {
                _ind = HandGuidanceIndicator.Create(HandGuidanceIndicator.HandGuidanceIndicatorType.Pointing, this, LevelManager.Level.BoardPositionIdToGlobalPosition(_currStep.Position.Value));
            }
            else if (_currStep.Type == TutorialStepType.PerformMove)
            {
                _ind = HandGuidanceIndicator.Create(HandGuidanceIndicator.HandGuidanceIndicatorType.Swiping, this, LevelManager.Level.BoardPositionIdToGlobalPosition(_currStep.Position.Value), LevelManager.Level.BoardPositionIdToGlobalPosition(_currStep.TargetPosition.Value));
            }

            if (_tutorialText.Text != _currStep.Text)
            {
                _tutorialText.Text = _currStep.Text;
                _tutorialText.Scale = new(.7f, .7f);
                TweenUtils.Pop(_tutorialText, 1, .5f);
            }
        }
        else
        {
            _tutorialText.Text = string.Empty;
        }
    }

    private void HandleSelectionStarted(Vector2I EaterPosId, Vector2I PossibleFoodPosId, bool IsCurrentlySelected)
    {
        if (_currStep?.Type == TutorialStepType.StartMove)
        {
            _currStepIdx++;
            SetupStep();
        }
    }

    private void HandleSelectionCancelled(Vector2I EaterPosId)
    {
        if (_prevStep?.Type == TutorialStepType.StartMove)
        {
            _currStepIdx--;
            SetupStep();
        }
    }

    private void HandleMovePerformed(Vector2I EaterPosId, Vector2I FoodPosId, FoodType FoodType, bool IsLast, bool IsHint)
    {
        if (_suggestUndo > 0)
        {
            _suggestUndo++;
            SetupStep();
        }
        else if (_currStep?.Type == TutorialStepType.PerformMove && _currStep.TargetPosition != FoodPosId)
        {
            _suggestUndo = 1;
            SetupStep();
        }
        else if (_currStep?.Type == TutorialStepType.PerformMove || _currStep.Type == TutorialStepType.TextOnly)
        {
            _currStepIdx++;
            SetupStep();
        }
    }

    private void HandleMoveUndone(Vector2I EaterPosId, Vector2I FoodPosId, FoodType FoodType, bool IsLast)
    {
        if (_suggestUndo > 0)
        {
            _suggestUndo--;
            SetupStep();
        }
        else if (_prevStep?.Type == TutorialStepType.PerformMove || _prevStep.Type == TutorialStepType.TextOnly)
        {
            _currStepIdx--;
            SetupStep();
        }
        else
        {
            var prevPerform = _steps.Select((step, i) => (Step: step, Index: i))
                .Where(idxStep => idxStep.Index < _currStepIdx && (idxStep.Step.Type == TutorialStepType.PerformMove || idxStep.Step.Type == TutorialStepType.TextOnly))
                .LastOrDefault();
            if (prevPerform != default)
            {
                _currStepIdx = prevPerform.Index;
                SetupStep();
            }
        }
    }
}