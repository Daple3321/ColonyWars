using UnityEngine;

[System.Serializable]
public class PlayerColony : Colony
{
    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        colonyZone = ZoneFactory.CreateTriggerZone(transform.position, Color.cyan, ZoneShape.Cylinder, colonyRadius, 10f);
        colonyZone.transform.position = transform.position;
        colonyZone.OnZoneEnter += x =>  {Debug.Log($"{x.gameObject.name} entered colony zone!");};
        //colonyZone.transform.SetParent(core.transform); // НУ И КАК ЭТО СДЕЛАТЬ ТО???
        EventBus.i.OnPlayerBuild += OnPlayerBuild;
    }
    
    public override BuildingPanel CreatePanel(RectTransform parentContainer)
    {
        GameObject go = Instantiate(GameAssets.colonyCenterPanel, parentContainer);
        ColonyCenterPanel panel = go.GetComponent<ColonyCenterPanel>();
        //panel.Init(this);
        return panel;
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
        if(Vector3.Distance(building.transform.position, transform.position) < colonyRadius)
        {
            AddBuilding(building);
        }
    }
}
