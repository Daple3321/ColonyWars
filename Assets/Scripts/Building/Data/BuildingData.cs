using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Scriptable Objects/Buildings/Building")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    [TextArea]
    public string description;
    public Sprite buildingIcon;
    
    public ItemRequirements craftPrice;
    
    // Unlock level or unlock type
    
    public bool insideColonyOnly = true;
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
        // if(insideColonyOnly)
        // {
        //     bool inColonyZone = false;
        //     foreach(Colony colony in ColoniesManager.i.playerColonies)
        //     {
        //         if(Vector3.Distance(projectionPos, colony.transform.position) < colony.colonyRadius)
        //         {
        //             //Debug.Log($"Inside colony! Dist: {Vector3.Distance(projectionPos, colony.core.transform.position)}", colony.core);
        //             inColonyZone = true;
        //         }
        //         else{
        //             //Debug.Log($"Outside colony! Dist: {Vector3.Distance(projectionPos, colony.core.transform.position)}", colony.core);
        //         }
        //     }
        //     return inColonyZone;
        // }
        
        if(insideColonyOnly)
        {
            bool inColonyZone = false;
            
            Vector3Int cellIndex = ColoniesManager.i.grid.WorldToCell(projectionPos);
            Cell hoveredCell = ColoniesManager.i.gridManager.GetCell(cellIndex.x, cellIndex.z);
            if(hoveredCell == null){ // если на нашли ячейку (за границей)
                inColonyZone = false;
            }
            
            if(hoveredCell.affiliation == Affiliation.Player){
                inColonyZone = true;
            }
            
            return inColonyZone;
        }
        
        return true;
    }
}
