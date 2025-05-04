using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Scriptable Objects/Buildings/Building")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public Sprite buildingIcon;
    
    public ItemRequirements craftPrice;
    
    // Unlock level or unlock type
    
    public float overlapRadius = 2.5f;
    public float maxBuildAngle = 25f;
    public GameObject prefab;
    
    public virtual Zone[] CreateProjectionZones()
    {
        Zone[] zones = null;
        // Zone overlapZone = ZoneFactory.CreateZone(
        //     PlayerAiming.worldMouseFollower.transform.position,
        //     GameAssets.colors.buildingOverlap,
        //     ZoneShape.Cylinder,
        //     overlapRadius);
        
        // overlapZone.transform.SetParent(PlayerAiming.worldMouseFollower.transform);
        // overlapZone.transform.localPosition = Vector3.zero;
        
        // zones[0] = overlapZone;
        
        return zones;
    }
    
    public virtual bool CheckBuildConditions(Vector3 projectionPos)
    {
        return true;
    }
}
