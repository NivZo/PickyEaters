using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Collection : PagedScreen<EaterDisplay>
{
    private const int ITEMS_PER_PAGE = 6;

    public override void _Ready()
    {
        base._Ready();
    }

    protected override List<EaterDisplay> CreateContents(int pageId)
    {
        return Enum.GetValues<EaterFace>()
            .Except(new EaterFace[1] { EaterFace.Hidden })
            .Chunk(ITEMS_PER_PAGE)
            .ElementAt(pageId)
            .Select((face, i) => 
            {
                GD.Print(i, "=", face);
                var eaterDisplay = GD.Load<PackedScene>("res://Entities/Eater/EaterDisplay.tscn").Instantiate<EaterDisplay>();
                eaterDisplay.Scale = new(3, 3);
                eaterDisplay.EaterFace = face;
                eaterDisplay.EaterType = EnumUtils.GetRandomValueExcluding(new EaterType[1] { EaterType.Hidden });
                var x = (i%2) switch
                {
                    0 => 360,
                    _ => 1080,
                };
                eaterDisplay.Position = new(x, 550 + 700 * (i/2));
                return eaterDisplay;
            })
            .ToList();
    }

    protected override int GetPageCount() => Mathf.CeilToInt((Enum.GetValues<EaterFace>().Length-1) / 6f);
}
