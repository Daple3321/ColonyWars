using UnityEngine;

[System.Serializable]
public class PlayerColony : Colony
{
    public PlayerColony(ColonyCore core, ColonyCenterData colonyData, Affiliation affiliation) : base(core, colonyData,affiliation)
    {
        colonyZone = ZoneFactory.CreateTriggerZone(core.transform.position, Color.cyan, ZoneShape.Cylinder, colonyRadius, 10f);
        colonyZone.transform.position = core.transform.position;
        colonyZone.OnZoneEnter += x =>  {Debug.Log($"{x.gameObject.name} entered colony zone!");};
        //colonyZone.transform.SetParent(core.transform); // НУ И КАК ЭТО СДЕЛАТЬ ТО???
        
        EventBus.i.OnPlayerBuild += OnPlayerBuild;
    }
    
    protected override void OnCoreDestroyed(Building core)
    {
        EventBus.i.OnPlayerBuild -= OnPlayerBuild;
        core.OnBuildingDestroyed -= OnCoreDestroyed;
        
        GameObject.Destroy(colonyZone.gameObject);
        foreach (Building building in buildings)
        {
            building.Death();
        }
    }
    
    public virtual void OnPlayerBuild(Building building)
    {
        if(Vector3.Distance(building.transform.position, core.transform.position) < colonyRadius)
        {
            AddBuilding(building);
        }
    }
}
