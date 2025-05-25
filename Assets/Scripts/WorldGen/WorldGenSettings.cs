using UnityEngine;

[CreateAssetMenu(fileName = "WorldGenSettings", menuName = "Scriptable Objects/WorldGenSettings")]
public class WorldGenSettings : ScriptableObject
{
    public float perstistance = 0.5f;
    public float lacunarity = 1.2f;
    public float _amplitude = 1f;
    public float _frequency = 3f;
    public int octaves = 1;

    public float maxNoiseHeight = 35f;
    public float minNoiseHeight = -3f;

    public int fallOffInner = 35;
    public float fallOffStrength = 10;
    
    [Multiline]
    public string desc;
}
