using UnityEngine;

public class BuildProjection : MonoBehaviour
{
    public BuildingData buildingData;
    
    private PlayerBuilding playerBuilding;
    
    public MeshRenderer[] meshes;
    
    public void Init(BuildingData building, PlayerBuilding playerBuilding)
    {
        this.playerBuilding = playerBuilding;
        this.buildingData = building;
        
        GameObject go = Instantiate(building.prefab, PlayerAiming.worldMouseFollower.transform.position, Quaternion.identity);
        go.transform.SetParent(transform);
        go.layer = 0;
        
        meshes = transform.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mesh in meshes){
            mesh.material.SetTexture("_BaseMap", null);
        }
        
        Destroy(go.GetComponent<Building>());
        Destroy(go.GetComponent<Collider>());
    }
    
    public void UpdateProjection(Vector3 playerPos) // build rules (distance, slope, obstacles)
    {
        if(playerBuilding.CheckBuildConditions(buildingData, transform.position)) // EVERY FRAME!!
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
    }
}
