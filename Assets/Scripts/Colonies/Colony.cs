using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Colony : Building
{
    public ColonyCenterData colonyData;
    
    public int level = 1;
    
    public float colonyRadius = 20f;
    
    //public ColonyCore core;
    public List<Building> buildings;
    public event Action<Building> OnBuildingAdded; // when new building is built in zone
    public event Action<Building> OnBuildingRemoved;
    
    //public Affiliation affiliation;
    public TriggerZone colonyZone;
    
    /*public virtual void Init(ColonyCore core, ColonyCenterData colonyData, Affiliation affiliation)
    {
        this.core = core;
        this.affiliation = affiliation;
        this.colonyRadius = colonyData.colonyRadius;
        buildings = new List<Building>();
        
        // if(affiliation == Affiliation.Player){
        //     colonyZone = ZoneFactory.CreateTriggerZone(core.transform.position, Color.cyan, ZoneShape.Cylinder, colonyRadius, 10f);
        //     colonyZone.transform.position = core.transform.position;
        //     //colonyZone.SetNoiseEffect(1);
        //     colonyZone.OnZoneEnter += x =>  {Debug.Log($"{x.gameObject.name} entered colony zone!");};
        //     //colonyZone.transform.SetParent(core.transform); // НУ И КАК ЭТО СДЕЛАТЬ ТО???
        // }
        // else{
        //     colonyZone = ZoneFactory.CreateTriggerZone(core.transform.position, Color.red, ZoneShape.Cylinder, colonyRadius, 10f);
        //     colonyZone.transform.position = core.transform.position;
        //     colonyZone.SetNoiseEffect(1);
        //     colonyZone.OnZoneEnter += x =>  {Debug.Log($"{x.gameObject.name} entered enemy zone!");};
        // }
        
        core.OnBuildingDestroyed += OnCoreDestroyed;
        EventBus.i.OnColonyCreated?.Invoke(this);
    }*/

    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        if(data is ColonyCenterData colonyCenterData){
            this.colonyData = colonyCenterData;
        } 
         
        this.colonyRadius = colonyData.colonyRadius;
        buildings = new List<Building>();
        OnBuildingDestroyed += OnCoreDestroyed;
        EventBus.i.OnColonyCreated?.Invoke(this);
    }
    
    public override void Death(){
        EventBus.i.OnColonyDestroyed?.Invoke(this);
        base.Death();
    }
    
    protected virtual void OnCoreDestroyed(Building core)
    {
        //GameObject.Destroy(colonyZone.gameObject);
        for(int i = 0; i < buildings.Count; i++)
        {
            buildings[i].Death();
            //GameObject.Destroy(building.gameObject);
        }
    }
    
    public virtual void AddBuilding(Building building)
    {
        if(building is Colony){
            return;
        }
        
        building.OnBuildingDestroyed += RemoveBuilding;
        buildings.Add(building);
        OnBuildingAdded?.Invoke(building);
    }
    
    
    public virtual void ClearAllModifiersFromSource(object source){}
    public virtual void RemoveBuilding(Building building)
    {
        if(building is Colony){
            return;
        }
        
        // Remove modifiers of that building
        //ClearAllModifiersFromSource(this);
        
        buildings.Remove(building);
        OnBuildingRemoved?.Invoke(building);
    }
}
