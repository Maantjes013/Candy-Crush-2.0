using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialCandyManager : MonoSingleton<SpecialCandyManager>
{
    [SerializeField] private List<BlockDecalConfig> specialEffectsConfig;

    private Dictionary<SpecialCandyType, BlockDecalConfig> configLookup;

    private void Awake()
    {
        configLookup = new Dictionary<SpecialCandyType, BlockDecalConfig>();
        foreach (var config in specialEffectsConfig)
        {
            if (!configLookup.ContainsKey(config.type))
            {
                configLookup.Add(config.type, config);
            }
        }
    }

    public Material GetDecalMaterial(SpecialCandyType specialCandyType)
    {
        return configLookup.ContainsKey(specialCandyType) ? configLookup[specialCandyType].decalMaterial : null;
    }

    public List<Candy> ActivateSpecialEffect(Candy specialCandy)
    {
        switch (specialCandy.specialCandyType)
        {
            case SpecialCandyType.None:
                break;

            case SpecialCandyType.Vertical:
                return DestroyVerticalCandy(specialCandy, MatchChecker.candyList);

            case SpecialCandyType.Horizontal:
                return DestroyHorizontalCandy(specialCandy, MatchChecker.candyList);

            case SpecialCandyType.VerticalAndHorizontal:
                return DestroyHorizontalAndVerticalCandy(specialCandy, MatchChecker.candyList);

            case SpecialCandyType.Bomb:
                return DestroyAllCandy(MatchChecker.candyList);
            default:
                break;
        }
        return null;
    }

    private List<Candy> DestroyHorizontalCandy(Candy specialCandy, List<Candy> candyList)
    {
        return candyList.Where(candy => candy.CurrentCoordinates.y == specialCandy.CurrentCoordinates.y).ToList();
    }

    private List<Candy> DestroyVerticalCandy(Candy specialCandy, List<Candy> candyList)
    {
        return candyList.Where(candy => candy.CurrentCoordinates.x == specialCandy.CurrentCoordinates.x).ToList();
    }

    private List<Candy> DestroyHorizontalAndVerticalCandy(Candy specialCandy, List<Candy> candyList)
    {
        return candyList.Where(candy =>
            candy.CurrentCoordinates.x == specialCandy.CurrentCoordinates.x ||
            candy.CurrentCoordinates.y == specialCandy.CurrentCoordinates.y).ToList();
    }

    private List<Candy> DestroyAllCandy(List<Candy> allCandy)
    {
        return new List<Candy>(allCandy);
    }
}
