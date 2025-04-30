using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBuilding : MonoBehaviour
{
    public bool buildMode = false;
    
    public List<BuildingData> availableBuildings;
    
    public BuildingData selectedBuilding;
    
    
    public float maxBuildDistance;
    public float maxBuildAngle;
    public LayerMask buildRaycastMask;
    public LayerMask buildingOverlapMask;
    private Controls controls;
    private Mouse mouse;
    private BuildProjection buildingProjection;
    private BuildUI buildUI;
    
    public event Action<List<BuildingData>> availableBuildingsChanged;
    void Awake(){enabled = false;}
    
    public void AddBuilding(BuildingData building)
    {
        availableBuildings.Add(building);
        availableBuildingsChanged?.Invoke(availableBuildings);
    }

    public void Init()
    {
        //availableBuildings = new List<BuildingData>();
        controls = GameAssets.controls;
        mouse = Mouse.current;
        
        GameObject go = Instantiate(GameAssets.buildCanvas);
        buildUI = go.transform.Find("BuildPanel").GetComponent<BuildUI>();
        buildUI.Init(this);
        
        buildUI.OnSlotClicked += SwitchBuildingMode;
        
        availableBuildingsChanged?.Invoke(availableBuildings);
        
        enabled = true;
    }

    void Update()
    {
        if(controls.Player.BuildMode.WasPressedThisFrame()){
            SwitchBuildUI();
        }
        
        if(buildMode){
            HandleBuilding();
        }
    }
    
    private void HandleBuilding()
    {
        RaycastHit hit;
        if(PlayerAiming.Raycast(buildRaycastMask, out hit))
        {
            PlayerAiming.worldMouseFollower.transform.position = hit.point;
            buildingProjection.UpdateProjection(transform.position);
        }
        
        // Rotation (ctrl + scrollwheel?)
        if(mouse.leftButton.IsPressed())
        {
            TryBuild(selectedBuilding, hit.point);
        }
        if(mouse.rightButton.IsPressed())
        {
            SwitchBuildingMode();
        }
    }
    
    public bool TryBuild(BuildingData building, Vector3 buildPos)
    {
        if(!CheckBuildConditions(building, buildPos)){
            Debug.LogWarning("Build conditions not met.");
            return false;
        }
        
        Building newBuilding = Instantiate(building.prefab, PlayerAiming.worldMouseFollower.transform.position, Quaternion.identity).GetComponent<Building>();
        StartCoroutine(newBuilding.Build());
        SwitchBuildingMode();
        
        return true;
    }
    
    public bool CheckBuildConditions(BuildingData building, Vector3 buildPos)
    {
        if(GameController.GetTerrainAngle(buildPos) >= maxBuildAngle)
            return false;
        if(Vector3.Distance(transform.position, buildPos) >= maxBuildDistance)
            return false;
        
        Collider[] hitColliders = Physics.OverlapSphere(buildPos, building.overlapRadius, buildingOverlapMask);
        if(hitColliders.Length > 0){
            //Debug.Log("Overlaping with building");
            return false;
        }
        
        return true;
    }

    public void SwitchBuildUI()
    {
        if(!buildUI.isActiveAndEnabled){
            buildUI.Show();
        }
        else if(buildUI.isActiveAndEnabled){
            buildUI.Hide();
        }
    }
    
    public void SwitchBuildingMode(BuildingData building = null)
    {
        if(buildMode){
            buildMode = false;
            selectedBuilding = null;
            DestroyBuildingProjection();
        }
        else if(!buildMode){
            SwitchBuildUI();
            buildMode = true;
            selectedBuilding = building;
            CreateBuildingProjection(building);
            
            EventBus.i.PlayerBuildModeEnter?.Invoke();
        } 
    }
    
    public void CreateBuildingProjection(BuildingData building)
    {
        GameObject projection = new GameObject("Build Projection");
        projection.transform.position = PlayerAiming.worldMouseFollower.transform.position;
        projection.transform.SetParent(PlayerAiming.worldMouseFollower.transform);

        buildingProjection = projection.AddComponent<BuildProjection>();
        buildingProjection.Init(building, this);
    }
    
    public void DestroyBuildingProjection(){
        Destroy(buildingProjection.gameObject);
    }
}
