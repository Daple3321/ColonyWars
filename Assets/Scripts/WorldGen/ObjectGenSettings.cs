using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectGenSettings", menuName = "Scriptable Objects/ObjectGenSettings")]
public class ObjectGenSettings : ScriptableObject
{
    public List<GenSettings> objects;
}

[System.Serializable]
public struct GenSettings
{
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