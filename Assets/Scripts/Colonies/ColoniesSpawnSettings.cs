using UnityEngine;

[CreateAssetMenu(fileName = "ColoniesSpawnSettings", menuName = "Scriptable Objects/Colonies Spawn Settings")]
public class ColoniesSpawnSettings : ScriptableObject
{
    public int colonies = 5;
    
    public float minColonyDistance = 80f;
    
    public ColonyCenterData[] coloniesPool;
}
