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
    
    [Space(10), Header("Building pools")]
    public int startingBuildings = 5;
    public int maxBuildings = 8;
    public int maxDefenses = 3;
    public List<BuildingData> resourcePool; // make them all weighted collections?
    public List<BuildingData> barracksPool; // make them all weighted collections?
    public List<BuildingData> storagePool; // make them all weighted collections?
    public List<BuildingData> defensePool; // make them all weighted collections?
    
    [Space(5), Header("Expansion")]
    public ExpansionSequence expansionSequence;
    private bool isDay = true;

    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        expansionSequence = ScriptableObject.Instantiate<ExpansionSequence>(expansionSequence);
        expansionSequence.Init(GameController.timeManager, this);
        
        EventBus.i.OnSunrise += ()=>{isDay = true;};
        EventBus.i.OnSunset += ()=>{isDay = false;};
        EventBus.i.OnMinuteChange += ()=> {expansionSequence.UpdateSequence(isDay);};
        
        // colonyZone = ZoneFactory.CreateTriggerZone(transform.position, Color.red, ZoneShape.Cylinder, colonyRadius, 10f);
        // colonyZone.transform.position = transform.position;
        // colonyZone.SetNoiseEffect(1);
        // colonyZone.OnZoneEnter += x =>  {Debug.Log($"{x.gameObject.name} entered enemy zone!");};
        
        // for(int i = 0; i < startingBuildings; i++)
        // {
        //     BuildingData randBuilding = buildingPool[Random.Range(0, buildingPool.Length)];
        //     Vector3 pointInRadius = GameController.RandomPointInCircleTerrain(new Vector2(transform.position.x, transform.position.z), 3, colonyRadius);
            
        //     Building b = GameController.objectGenerator.CreateBuilding_Rules(randBuilding, pointInRadius);
        //     if(b != null)
        //     {
        //         b.Init(randBuilding);
        //         StartCoroutine(b.Build());
                
        //         AddBuilding(b);
        //     }
        // }
        for(int i = 0; i < startingBuildings; i++)
        {
            BuildingData randBuilding = resourcePool[Random.Range(0, resourcePool.Count)];
            Vector3 pointInCell = ColoniesManager.i.gridManager.RandomPointInCell(cellIndex.x, cellIndex.z);
            
            Building b = GameController.objectGenerator.CreateBuilding_Rules(randBuilding, pointInCell);
            if(b != null)
            {
                b.Init(randBuilding);
                StartCoroutine(b.Build());
                
                AddBuilding(b);
            }
        }
        
        EventBus.i.OnSunrise += OnSunrise;
    }

    void Update()
    {
        //expansionSequence.UpdateSequence(isDay);
    }
    
    public void PerformAction(ColonyAction action)
    {
        HandleAction(action.actionType);
        
        Debug.Log($"{gameObject.name} Performed {action.actionType}", gameObject);
    }
    
    protected virtual void HandleAction(ColonyActionType actionType)
    {
        switch(actionType){
        case ColonyActionType.Expand:
            Expand();
        break;
        
        case ColonyActionType.Capture:
            
        break;
        
        case ColonyActionType.RaidSquad:
            
        break;
        
        case ColonyActionType.Resources:
            PerformBuildAction(actionType);
        break;
        
        case ColonyActionType.Storage:
            PerformBuildAction(actionType);
        break;
        
        case ColonyActionType.Defense:
            PerformBuildAction(actionType);
        break;
        
        case ColonyActionType.Barracks:
            PerformBuildAction(actionType);
        break;
        
        }
    }
    
    protected virtual void PerformBuildAction(ColonyActionType buildAction)
    {
        Cell c = capturedCells[Random.Range(0, capturedCells.Count)];
        Vector3 pos = ColoniesManager.i.gridManager.RandomPointInCell(c);
        
        switch(buildAction){
        case ColonyActionType.Resources:
            Build(resourcePool[Random.Range(0, resourcePool.Count)], pos);
        break;
        
        case ColonyActionType.Storage:
            Build(storagePool[Random.Range(0, storagePool.Count)], pos);
        break;
        
        case ColonyActionType.Barracks:
            Build(barracksPool[Random.Range(0, barracksPool.Count)], pos);
        break;
        
        case ColonyActionType.Defense:
            Build(defensePool[Random.Range(0, defensePool.Count)], pos);
        break;
        }
    }
    protected void Build(BuildingData building, Vector3 pos)
    {
        if(buildings.Count < maxBuildings)
        {
            Building b = GameController.objectGenerator.CreateBuilding_Rules(building, pos);
            if(b != null)
            {
                b.Init(building);
                StartCoroutine(b.Build());
                
                AddBuilding(b);
            }
        }
        else{
            // level up
        }
    }

    public override void Death(){
        EventBus.i.OnColonyDestroyed?.Invoke(this);
        EventBus.i.OnSunrise -= OnSunrise;
        EventBus.i.OnSunrise -= ()=>{isDay = true;};
        EventBus.i.OnSunset -= ()=>{isDay = false;};
        EventBus.i.OnMinuteChange -= ()=> {expansionSequence.UpdateSequence(isDay);};
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
        Vector2Int newCell = ColoniesManager.i.gridManager._ClosestDifferentCell(cellIndex.x, cellIndex.z);
        CaptureCellInstant(newCell.x, newCell.y);
        //ColoniesManager.i.gridManager.cells[newCell.x, newCell.y].Capture(Affiliation.Enemy);
    }
}
