using System.Collections.Generic;
using UnityEngine;

public class BuildProjection : MonoBehaviour
{
    public BuildingData buildingData;
    
    private PlayerBuilding playerBuilding;
    
    public MeshRenderer[] meshes;
    
    public Zone cellZone;
    
    public void Init(BuildingData building, PlayerBuilding playerBuilding)
    {
        this.playerBuilding = playerBuilding;
        this.buildingData = building;
        previousResources = new();
        
        GameObject go = Instantiate(building.prefab, PlayerAiming.worldMouseFollower.transform.position, Quaternion.identity);
        go.transform.SetParent(transform);
        go.layer = 0;
        
        meshes = transform.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mesh in meshes){
            mesh.material.SetTexture("_BaseMap", null);
        }
        
        cellZone = ZoneFactory.CreateZone(
            transform.position,
            GameAssets.colors.playerBorder,
            ZoneShape.Box,
            ColoniesManager.i.gridManager.cellHalf,
            35
        );
        //cellZone.transform.SetParent(transform);
        
        Destroy(go.GetComponent<Building>());
        if(go.TryGetComponent(out Defense def)){
            Destroy(def);
        }
        Destroy(go.GetComponent<Collider>());
    }
    
    public List<int> previousResources = new();
    public void UpdateProjection(Vector3 playerPos) // build rules (distance, slope, obstacles)
    {
        if(playerBuilding.CheckBuildConditions(buildingData, transform.position) && buildingData.CheckBuildConditions(transform.position, this)) // EVERY FRAME!!
        {
            foreach(MeshRenderer mesh in meshes){
                mesh.material.SetColor("_BaseColor", Color.green);
            }
        }
        else{
            foreach(MeshRenderer mesh in meshes){
                mesh.material.SetColor("_BaseColor", Color.red);
            }
        }
        
        Vector3Int projectionGridPos = ColoniesManager.i.grid.WorldToCell(transform.position);
        cellZone.transform.position = ColoniesManager.i.grid.GetCellCenterWorld(projectionGridPos);
    }
}
