using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Colony
{
    public int level = 1;
    
    public float colonyRadius = 20f;
    
    public ColonyCore core;
    public List<Building> buildings;
    
    public Affiliation affiliation;
    public TriggerZone colonyZone;
    public Colony(ColonyCore core, ColonyCenterData colonyData, Affiliation affiliation)
    {
        this.core = core;
        this.affiliation = affiliation;
        this.colonyRadius = colonyData.colonyRadius;
        buildings = new List<Building>();
        
        colonyZone = ZoneFactory.CreateTriggerZone(core.transform.position, Color.cyan, ZoneShape.Cylinder, colonyRadius, 10f);
        colonyZone.transform.position = core.transform.position;
        //colonyZone.SetNoiseEffect(1);
        
        colonyZone.OnZoneEnter += x =>  {Debug.Log($"{x.gameObject.name} entered colony zone!");};
        //colonyZone.transform.SetParent(core.transform); // НУ И КАК ЭТО СДЕЛАТЬ ТО???
        
        EventBus.i.OnColonyCreated?.Invoke(this);
    }
    
}
