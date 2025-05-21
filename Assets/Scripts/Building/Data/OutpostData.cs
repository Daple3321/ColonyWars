using UnityEngine;

[CreateAssetMenu(fileName = "Outpost", menuName = "Scriptable Objects/Buildings/Outpost")]
public class OutpostData : BuildingData
{
    public float captureSpeed = 1f;
    
    public Affiliation affiliation;

    public override bool CheckBuildConditions(Vector3 projectionPos)
    {
        if (!base.CheckBuildConditions(projectionPos)){
            return false;
        }
        
        Vector3Int cellIndex = ColoniesManager.i.grid.WorldToCell(projectionPos);
        Cell hoveredCell = ColoniesManager.i.gridManager.GetCell(cellIndex.x, cellIndex.z);
        if(hoveredCell.whoIsCapturing != Affiliation.None){
            return false;
        }
        
        return true;
    }
}
