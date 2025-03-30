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

    public override void _Ready()
    {
        EventManager.MoveSelectionStarted += HandleSelectionStarted;
        EventManager.MoveSelectionCancelled += HandleSelectionCancelled;
        EventManager.MovePerformed += HandleMovePerformed;

        _tutorialText = GetNode<RichTextLabel>("TutorialText");

        SetupStep();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventManager.MoveSelectionStarted -= HandleSelectionStarted;
        EventManager.MoveSelectionCancelled -= HandleSelectionCancelled;
        EventManager.MovePerformed -= HandleMovePerformed;
    }

    public enum TutorialStepType
    {
        StartMove,
        PerformMove,
    }

    public record TutorialStep(TutorialStepType Type, string Text, Vector2 Position, Vector2? TargetPosition = null);

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
        
        if (_currStep != null)
        {
            if (_currStep?.Type == TutorialStepType.StartMove)
            {
                _ind = HandGuidanceIndicator.CreatePointing(this, _currStep.Position);
            }
            else if (_currStep?.Type == TutorialStepType.PerformMove)
            {
                _ind = HandGuidanceIndicator.CreateSwiping(this, _currStep.Position, _currStep.TargetPosition.Value);
            }

            _tutorialText.Text = _currStep.Text;
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
        if (_currStep?.Type == TutorialStepType.PerformMove)
        {
            _currStepIdx++;
            SetupStep();
        }
    }
}