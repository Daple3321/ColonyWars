using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "ColoniesSpawnSettings", menuName = "Scriptable Objects/Colonies Spawn Settings")]
public class ColoniesSpawnSettings : ScriptableObject
{
    public int colonies = 1;
    
    public Vector3Int spawnCell;
    
    //public ColonyCenterData[] coloniesPool;
    
    [SerializedDictionary("Colony", "Chance to spawn")]
    public SerializedDictionary<ColonyCenterData, float> colonyPool;
    
    public Vector2Int campsRange;
    [SerializedDictionary("Camp", "Chance to spawn")]
    public SerializedDictionary<ColonyCenterData, float> campPool;
}
