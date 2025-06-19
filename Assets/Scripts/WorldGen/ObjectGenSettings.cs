using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectGenSettings", menuName = "Scriptable Objects/ObjectGenSettings")]
public class ObjectGenSettings : ScriptableObject
{
    [SubclassSelector, SerializeReference] public List<GenSettings> objects;
}

[System.Serializable]
public abstract class GenSettings
{
    public string objName;
    public SpawnType spawnType;
    public SpawnRule[] spawnRules;
    //public Vector2 spawnRuleValue;
    
    public int spawnAmount;
    [Range(0f, 1f)]
    public float spawnChance;
    
    //[Range(0f, 1f)]
    public bool alignToGround;
    
    public bool randomizeScale;
    public Vector2 scaleRange;
    
    public GameObject prefab;
}

[System.Serializable]
public class ObjGenSettings : GenSettings
{
    
}

[System.Serializable]
public class ResourceGenSettings : GenSettings
{
    [Space(7), Header("Resource Settings")]
    public ItemData resource;
    public Vector2Int amountRange;
    public int clicksToGather;
}

[System.Serializable]
public struct SpawnRule
{
    public SpawnRuleType spawnRuleType;
    public Vector2 valueRange;
}

public enum SpawnRuleType : byte
{
    Height,
    Angle,
    Position,
}

public enum SpawnType : byte
{
    Linear, // loop through all pixels in terrain
    Raycast,
}