using UnityEngine;

[CreateAssetMenu(fileName = "Colony Center", menuName = "Scriptable Objects/Buildings/Colony Center")]
public class ColonyCenterData : BuildingData
{
    public bool canOverlapOtherColonies = false;
    public float colonyRadius = 20f;
    
    public override Zone[] CreateProjectionZones()
    {
        //Zone[] zones = new Zone[1];
        Zone[] zones = null;
        
        // Zone coloneZone = ZoneFactory.CreateZone(
        //     PlayerAiming.worldMouseFollower.transform.position,
        //     Color.cyan,
        //     ZoneShape.Cylinder,
        //     colonyRadius);
        
        // coloneZone.transform.SetParent(PlayerAiming.worldMouseFollower.transform);
        // coloneZone.transform.localPosition = Vector3.zero;
        
        // zones[0] = coloneZone;
        
        return zones;
    }
    
    public override bool CheckBuildConditions(Vector3 projectionPos)
    {
        if(!base.CheckBuildConditions(projectionPos)){
            return false;
        }
        
        Vector3Int cellIndex = ColoniesManager.i.grid.WorldToCell(projectionPos);
        Cell hoveredCell = ColoniesManager.i.gridManager.GetCell(cellIndex.x, cellIndex.z);
        if(hoveredCell == null){
            return false;
        }
        if(hoveredCell.whoIsCapturing == Affiliation.Enemy){
            return false;
        }
        if(hoveredCell.affiliation == Affiliation.Enemy){
            return false;
        }
        
        // Old radius overlap checks
        // if(!canOverlapOtherColonies)
        // {
        //     bool overlappingWithColony = false;
        //     foreach(Colony colony in ColoniesManager.i.playerColonies)
        //     {
        //         float colonyDistance = Vector3.Distance(projectionPos, colony.transform.position);
        //         float radiusSum = colonyRadius + colony.colonyRadius;
        //         if(colonyDistance < radiusSum)
        //         {
        //             //Debug.Log($"Overlapping player colony! Dist: {Vector3.Distance(projectionPos, colony.core.transform.position)}", colony.core);
        //             overlappingWithColony = true;
        //         }
        //         else{
        //             //Debug.Log($"Outside any player colonies! Dist: {Vector3.Distance(projectionPos, colony.core.transform.position)}", colony.core);
        //         }
        //     }
        //     if(!overlappingWithColony){
        //         //Debug.Log($"Outside any player colonies!");
        //     }
            
        //     foreach(Colony colony in ColoniesManager.i.enemyColonies)
        //     {
        //         float colonyDistance = Vector3.Distance(projectionPos, colony.transform.position);
        //         float radiusSum = colonyRadius + colony.colonyRadius;
                
        //         if(colonyDistance < radiusSum)
        //         {
        //             //Debug.Log($"Overlapping with enemy colony! Dist: {Vector3.Distance(projectionPos, colony.core.transform.position):1F}", colony.core);
        //             overlappingWithColony = true;
        //         }
        //         else{
        //             //Debug.Log($"Outside colony! Dist: {Vector3.Distance(projectionPos, colony.core.transform.position)}", colony.core);
        //         }
        //     }
        //     if(!overlappingWithColony){
        //         //Debug.Log($"Outside ANY colonies!");
        //     }
            
        //     return !overlappingWithColony; // ну это тупость так менять
        // }
        
        
        return true;
    }
}
