using System.Collections.Generic;
using UnityEngine;

public class DecalManager : MonoBehaviour
{
    [SerializeField] private List<BlockDecalConfig> decalConfigs;

    private Dictionary<SpecialCandyType, BlockDecalConfig> configLookup;

    private void Awake()
    {
        MatchChecker.decalManager = this;
        // Build a lookup dictionary for fast access
        configLookup = new Dictionary<SpecialCandyType, BlockDecalConfig>();
        foreach (var config in decalConfigs)
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
}
