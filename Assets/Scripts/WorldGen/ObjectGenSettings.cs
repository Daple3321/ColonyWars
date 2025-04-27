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
    public SpawnRule spawnRule;
    public Vector2 spawnRuleValue;
    
    public int spawnAmount;
    public float spawnChance;
    
    public GameObject prefab;
}

public enum SpawnRule : byte
{
    Height,
    Steepness,
    Normal,
    Position,
}