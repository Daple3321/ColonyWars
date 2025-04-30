using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Building", menuName = "Scriptable Objects/Building")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public Sprite buildingIcon;
    
    // Crafting prices (Resources)
    // Unlock level or unlock type
    
    public float overlapRadius = 2.5f;
    public float maxBuildAngle = 25f;
    public GameObject prefab;
}
