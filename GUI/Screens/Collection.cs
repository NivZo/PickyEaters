using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Collection : PagedScreen<EaterShowcase>
{
    private const int ITEMS_PER_PAGE = 6;

    public override void _Ready()
    {
        base._Ready();
    }

    protected override List<EaterShowcase> CreateContents(int pageId)
    {
        return Enum.GetValues<EaterFace>()
            .Except(new EaterFace[1] { EaterFace.Hidden })
            .Chunk(ITEMS_PER_PAGE)
            .ElementAt(pageId)
            .Select((face, i) => 
            {
                var eaterShowcase = GD.Load<PackedScene>("res://Entities/Eater/EaterShowcase.tscn").Instantiate<EaterShowcase>();
                eaterShowcase.Setup();
                eaterShowcase.Display.BaseScale = 3;
                eaterShowcase.Display.EaterFace = face;
                eaterShowcase.Display.EaterType = EnumUtils.GetRandomValueExcluding(new EaterType[1] { EaterType.Hidden });
                eaterShowcase.Display.Setup();
                eaterShowcase.RandomFace = false;
                var x = (i%2) switch
                {
                    0 => 360,
                    _ => 1080,
                };
                eaterShowcase.Position = new(x, 550 + 700 * (i/2));
                return eaterShowcase;
            })
            .ToList();
    }

    protected override int GetPageCount() => Mathf.CeilToInt((Enum.GetValues<EaterFace>().Length-1) / 6f);
}
