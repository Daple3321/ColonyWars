using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyColony : Colony
{
    /*public EnemyColony(EnemyColonyCore core, ColonyCenterData colonyData, Affiliation affiliation) : base(core, colonyData, affiliation)
    {
        colonyZone = ZoneFactory.CreateTriggerZone(core.transform.position, Color.red, ZoneShape.Cylinder, colonyRadius, 10f);
        colonyZone.transform.position = core.transform.position;
        colonyZone.SetNoiseEffect(1);
        colonyZone.OnZoneEnter += x =>  {Debug.Log($"{x.gameObject.name} entered enemy zone!");};
        
        EventBus.i.OnSunrise += OnSunrise;
    }*/
    public int startingBuildings = 5;
    public int maxBuildings = 8;
    public BuildingData[] buildingPool;
    
    public int maxDefenses = 3;
    public BuildingData[] defensePool;

    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        colonyZone = ZoneFactory.CreateTriggerZone(transform.position, Color.red, ZoneShape.Cylinder, colonyRadius, 10f);
        colonyZone.transform.position = transform.position;
        colonyZone.SetNoiseEffect(1);
        colonyZone.OnZoneEnter += x =>  {Debug.Log($"{x.gameObject.name} entered enemy zone!");};
        
        for(int i = 0; i < startingBuildings; i++)
        {
            BuildingData randBuilding = buildingPool[Random.Range(0, buildingPool.Length)];
            Vector3 pointInRadius = GameController.RandomPointInCircleTerrain(new Vector2(transform.position.x, transform.position.z), 3, colonyRadius);
            
            Building b = GameController.objectGenerator.CreateBuilding_Rules(randBuilding, pointInRadius);
            if(b != null)
            {
                b.Init(randBuilding);
                StartCoroutine(b.Build());
                
                AddBuilding(b);
            }
        }
        
        EventBus.i.OnSunrise += OnSunrise;
    }
    
    public override void Death(){
        EventBus.i.OnColonyDestroyed?.Invoke(this);
        base.Death();
    }
    
    public override BuildingPanel CreatePanel(RectTransform parentContainer)
    {
        return null;
    }
    
    protected void OnSunrise()
    {
        Expand();
    }
    
    protected void Expand()
    {
        
    }
}
