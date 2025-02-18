using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

public class DragSelectComponent<TCategory> : IDisposable
{
    public bool IsSelected { get => _isSelected; }
    private static List<DragSelectComponent<TCategory>> _selectComponents = new();
    private bool _isSelected = false;
    private bool _isSelectable = true;
    private Func<bool> _selectCondition;
    private CollisionObject2D _collider;
    private List<Action> _onSelectAction = new();
    private List<Action> _onDeselectAction = new();

    public DragSelectComponent(CollisionObject2D collider, Func<bool> selectCondition = null)
    {
        _collider = collider;
        _collider.InputEvent += HandleInput;
        
        _selectCondition = selectCondition ?? (() => true);

        _selectComponents.Add(this);
    }

    public event Action Select
    {
        add => _onSelectAction.Add(value);
        remove => _onSelectAction.Remove(value);
    }

    public event Action Deselect
    {
        add => _onDeselectAction.Add(value);
        remove => _onDeselectAction.Remove(value);
    }

    public void ManualDeselection() => HandleDeselection();
    
    private void HandleInput(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.ButtonIndex == MouseButton.Left)
        {
            if (inputEventMouseButton.IsReleased() && _isSelected)
            {
                HandleDeselection();
            }
            
            else if (inputEventMouseButton.IsPressed() && !_isSelected && _selectCondition.Invoke())
            {
                HandleSelection();
            }
        }
    }

    private void HandleSelection()
    {
        if (_isSelectable && !_selectComponents.Any(sel => sel.IsSelected))
        {
            _isSelected = true;

            _onSelectAction.ForEach(action => action.Invoke());
        }
    }

    private void HandleDeselection()
    {
        _isSelected = false;

        _onDeselectAction.ForEach(action => action.Invoke());
    }

    public void Dispose()
    {
        if (_selectComponents.Contains(this))
        {
            _selectComponents.Remove(this);
        }
        
        GC.SuppressFinalize(this);
    }
}


