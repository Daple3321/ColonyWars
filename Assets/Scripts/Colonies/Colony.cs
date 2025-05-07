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
    public Zone colonyZone;
    public Colony(ColonyCore core, Affiliation affiliation)
    {
        this.core = core;
        this.affiliation = affiliation;
        buildings = new List<Building>();
        
        colonyZone = ZoneFactory.CreateTriggerZone(core.transform.position, Color.cyan, ZoneShape.Cylinder, colonyRadius, 10f);
        colonyZone.transform.SetParent(core.transform);
        
        EventBus.i.OnColonyCreated?.Invoke(this);
    }
    
}
