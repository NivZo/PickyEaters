using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Collection : PagedScreen<EaterCollectionShowcase>
{
    private const int ITEMS_PER_PAGE = 6;

    public override void _Ready()
    {
        base._Ready();
        BackgroundManager.ChangeColor(NamedColor.Blue.GetColor(), lightenFactor: .45f);
    }

    protected override List<EaterCollectionShowcase> CreateContents(int pageId)
    {
        var colors = Enum.GetValues<EaterType>().Except(new[] { EaterType.Hidden }).ToList().Shuffle().Take(6);
        return Enum.GetValues<EaterFace>()
            .Except(new EaterFace[1] { EaterFace.Hidden })
            .Select(face => face.GetEaterResource())
            .OrderByDescending(eaterResource => SaveManager.ActiveSave.UnlockedFaces.Contains(eaterResource.EaterFace)).ThenBy(eaterResource => eaterResource.EaterRarity).ThenBy(eaterResource => (int)eaterResource.EaterFace)
            .Chunk(ITEMS_PER_PAGE)
            .ElementAt(pageId)
            .Select((eaterResource, i) => 
            {
                var eaterShowcase = GD.Load<PackedScene>("res://Entities/Eater/EaterCollectionShowcase.tscn").Instantiate<EaterCollectionShowcase>();
                eaterShowcase.Setup();
                eaterShowcase.Display.BaseScale = 2f;
                eaterShowcase.Display.EaterFace = eaterResource.EaterFace;
                eaterShowcase.Display.EaterType = colors.ElementAt(i);
                eaterShowcase.RandomFace = false;
                eaterShowcase.RandomColor = false;
                eaterShowcase.Display.Setup();
                var x = (i%2) switch
                {
                    0 => 360,
                    _ => 1080,
                };
                eaterShowcase.Position = new(x, 480 + 640 * (i/2));
                return eaterShowcase;
            })
            .ToList();
    }

    protected override int GetPageCount() => Mathf.CeilToInt((Enum.GetValues<EaterFace>().Length-1) / 6f);
}
