using System;
using Godot;

public partial class Collection : Node
{
    public static Collection Instance;

    private Scrollable _scrollable;

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        
        _scrollable = GetNode<Scrollable>("GUILayer/Scrollable");

        var faces = Enum.GetValues<EaterFace>();

        for (int i = 0; i < faces.Length; i++)
        {
            var eaterDisplay = GD.Load<PackedScene>("res://Entities/Eater/EaterDisplay.tscn").Instantiate<EaterDisplay>();
            eaterDisplay.Scale = new(3, 3);
            var x = (i%2) switch
                {
                    1 => 360,
                    _ => 1080,
                };
            eaterDisplay.Position = new(x, 300 + 700 * ((i-1)/2));
            eaterDisplay.EaterFace = faces[i];
            eaterDisplay.EaterType = EnumUtils.GetRandomValueExcluding(new EaterType[1] { EaterType.Hidden });

            _scrollable.AddChildToScrollableContent(eaterDisplay);
            eaterDisplay.Setup();
        }
    }

    public void SetColor(EaterType eaterType)
    {
        foreach (var item in _scrollable.Items)
        {
            if (item is EaterDisplay eaterDisplay)
            {
                eaterDisplay.EaterType = eaterType;
                eaterDisplay.Setup();
            }
        }
    }
}
