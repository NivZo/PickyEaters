using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

public static class UnlockManager
{
    private static Dictionary<Rarity, int> _rarityWeights = new()
    {
        { Rarity.Common, 10 },
        { Rarity.Rare, 5 },
        { Rarity.Epic, 2 },
        { Rarity.Legendary, 1 },
    };
    public static EaterFace GetEaterFaceToUnlock()
    {
        // Guranteed to unlock an epic first
        if (SaveManager.ActiveSave.UnlockedFaces.Count == 1)
        {
            return Enum.GetValues<EaterFace>()
                .Except(SaveManager.ActiveSave.UnlockedFaces.Concat(new[] { EaterFace.Hidden }))
                .Where(face => face.GetEaterResource().EaterRarity == Rarity.Epic)
                .ToList()
                .Shuffle()
                .FirstOrDefault();
        }

        var weightedFacesAvailable = Enum.GetValues<EaterFace>()
            .Except(SaveManager.ActiveSave.UnlockedFaces.Concat(new[] { EaterFace.Hidden }))
            .Select(face => (Face: face, Rarity: face.GetEaterResource().EaterRarity))
            .OrderBy(faceWithRarity => faceWithRarity.Rarity)
            .Select(faceWithRarity => (faceWithRarity.Face, Weight: _rarityWeights[faceWithRarity.Rarity]));
        
        var sum = weightedFacesAvailable.Sum(wf => wf.Weight);
        var randomValue = RandomUtils.RandomInRange(0, sum);
        var cumulativeSum = 0;

        foreach (var (face, weight) in weightedFacesAvailable)
        {
            cumulativeSum += weight;
            if (randomValue <= cumulativeSum)
            {
                return face;
            }
        }

        return weightedFacesAvailable.FirstOrDefault().Face;
    }
}