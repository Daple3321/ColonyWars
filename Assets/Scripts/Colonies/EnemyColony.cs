using System.Collections.Generic;
using UnityEngine;
using static ColonyStatType;
using AYellowpaper.SerializedCollections;

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
    [Space(10), Header("Stats")]
    // public ColonyStat maxBuildings;
    // public ColonyStat maxDefenses;
    // public ColonyStat maxUnits;
    // public ColonyStat maxUnitsLevel;
    // public ColonyStat unitsSpawnSpeed;
    public int startingBuildings = 5;
    
    [SerializedDictionary("ColonyStatType", "ColonyStat")]
    public SerializedDictionary<ColonyStatType, Stat> stats;
    
    
    [Space(10), Header("Building pools")]
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
        
        stats = new SerializedDictionary<ColonyStatType, Stat>();
        stats[maxBuildings] = new(10);
        stats[maxDefenses] = new(5);
        stats[maxUnits] = new(10);
        stats[maxUnitsLevel] = new(1);
        stats[unitsSpawnSpeed] = new(10);
        // maxBuildings = new(10);
        // maxDefenses = new(5);
        // maxUnits = new(10);
        // maxUnitsLevel = new(1);
        // unitsSpawnSpeed = new(10);
        
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
                
                ApplyModifiersToStats(b.GetComponent<EnemyBuilding>().GetModifiers());
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
            //if(maxDefenses.Value)
            Build(barracksPool[Random.Range(0, barracksPool.Count)], pos);
        break;
        
        case ColonyActionType.Defense:
            Build(defensePool[Random.Range(0, defensePool.Count)], pos);
        break;
        }
    }
    protected void Build(BuildingData building, Vector3 pos)
    {
        if(buildings.Count < stats[maxBuildings].Value)
        {
            Building b = GameController.objectGenerator.CreateBuilding_Rules(building, pos);
            if(b != null)
            {
                b.Init(building);
                StartCoroutine(b.Build());
                
                AddBuilding(b);
                
                ApplyModifiersToStats(b.GetComponent<EnemyBuilding>().GetModifiers());
                
                foreach(var stat in stats)
                {
                    Debug.Log($"{stat.Key} = {stat.Value.Value}");
                }
            }
        }
        else{
            // level up
        }
    }
    
    public void ApplyModifiersToStats(Dictionary<ColonyStatType, StatModifier> modifiers)
    {
        foreach(var mod in modifiers)
        {
            stats[mod.Key].AddModifier(mod.Value);
            Debug.Log($"Applied mod {mod.Key}:{mod.Value.Value}");
        }
        
    }
    public override void ClearAllModifiersFromSource(object source)
    {
        foreach(var stat in stats)
        {
            stat.Value.RemoveAllModifiersFromSource(source);
            //Debug.Log($"Cleared mods from {stat.Key} = {stat.Value.Value}");
        }
    }
    
    public override void RemoveBuilding(Building building)
    {
        // Remove modifiers of that building
        ClearAllModifiersFromSource(building);
        
        base.RemoveBuilding(building);
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
