using System;
using System.Linq;
using Godot;

public partial class EaterShowcase : Node2D
{
    [Export] public bool RandomFace = true;
    [Export] public bool RandomColor = true;
    public TargetPositionComponent TargetPositionComponent;
    public EaterDisplay Display;

    private SelectComponent<Eater> _selectComponent;

    public override void _Ready()
    {
        base._Ready();
        
        Setup();

        _selectComponent = Display.SelectComponent;
        _selectComponent.Select += OnSelect;
        _selectComponent.Deselect += OnDeselect;
    }

    public void Setup()
    {
        Display ??= GetNode<EaterDisplay>("EaterDisplay");

        if (RandomFace)
        {
            Display.EaterFace = GetRandomEaterFace();
        }

        if (RandomColor)
        {
            Display.EaterType = GetRandomEaterType();
        }
        
        if (RandomFace || RandomColor)
        {
            Display.Setup();
        }
    }

    private void OnSelect()
    {
        ZIndex = 1;
    }

    private void OnDeselect()
    {
        Setup();
        ZIndex = 0;
    }

    private EaterFace GetRandomEaterFace() => EnumUtils.GetRandomValueOutOf(SaveManager.ActiveSave.UnlockedFaces.ToArray());

    private EaterType GetRandomEaterType() => EnumUtils.GetRandomValueOutOf(Enum.GetValues<EaterType>().Except(new EaterType[2] { EaterType.Hidden, Display.EaterType }).ToArray());

}
