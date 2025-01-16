using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockDecalConfig", menuName = "Game/BlockDecalConfig")]
public class BlockDecalConfig : ScriptableObject
{
    public SpecialCandyType type;
    public Material decalMaterial;
}
