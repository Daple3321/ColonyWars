using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyColony : Colony
{
    public EnemyColony(ColonyCore core, ColonyCenterData colonyData, Affiliation affiliation) : base(core, colonyData, affiliation)
    {
        colonyZone = ZoneFactory.CreateTriggerZone(core.transform.position, Color.red, ZoneShape.Cylinder, colonyRadius, 10f);
        colonyZone.transform.position = core.transform.position;
        colonyZone.SetNoiseEffect(1);
        colonyZone.OnZoneEnter += x =>  {Debug.Log($"{x.gameObject.name} entered enemy zone!");};
        
        EventBus.i.OnSunrise += OnSunrise;
    }
    
    public List<GameObject> buildingPool;
    
    protected void OnSunrise()
    {
        Expand();
    }
    
    protected void Expand()
    {
        
    }
}
