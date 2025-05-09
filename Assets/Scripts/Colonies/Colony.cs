using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Colony
{
    public int level = 1;
    
    public float colonyRadius = 20f;
    
    public ColonyCore core;
    public List<Building> buildings;
    public event Action<Building> OnBuildingAdded; // when new building is built in zone
    public event Action<Building> OnBuildingRemoved;
    
    public Affiliation affiliation;
    public TriggerZone colonyZone;
    public Colony(ColonyCore core, ColonyCenterData colonyData, Affiliation affiliation)
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
    }
    
    protected virtual void OnCoreDestroyed(Building core)
    {
        GameObject.Destroy(colonyZone.gameObject);
        foreach (Building building in buildings)
        {
            building.Death();
            //GameObject.Destroy(building.gameObject);
        }
    }
    
    public virtual void AddBuilding(Building building)
    {
        if(building is ColonyCore){
            return;
        }
        
        buildings.Add(building);
        OnBuildingAdded?.Invoke(building);
    }
    
    public virtual void RemoveBuilding(Building building)
    {
        if(building is ColonyCore){
            return;
        }
        
        buildings.Remove(building);
        OnBuildingRemoved?.Invoke(building);
    }
}
