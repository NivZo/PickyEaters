using Godot;
using System;

public class ScrollComponent
{
    private Area2D _scrollArea;
    private Node2D _scrollContent;
    private Func<bool> _selectCondition;
}