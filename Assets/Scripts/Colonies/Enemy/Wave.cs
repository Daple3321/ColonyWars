using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "AI/Raid Wave")]
public class Wave : ScriptableObject
{
    [SerializedDictionary("Unit", "Chance to spawn")]
    public SerializedDictionary<UnitData, float> unitPool;
    
    public Vector2Int unitAmountRange;
    
    [SerializedDictionary("Stat", "Modifier")]
    public SerializedDictionary<EntityStatType, StatModifier> unitModifiers;
    
    [Range(0f, 1f)]
    public float colonyInfluence_UnitAmount = 0f;
    [Range(0f, 1f)]
    public float colonyInfluence_UnitLevel = 0f;
}
