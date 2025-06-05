using System.Collections.Generic;
using UnityEngine;
using static ColonyStatType;
using AYellowpaper.SerializedCollections;
using System;
using Random = UnityEngine.Random;
using System.Text;

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
    public int startingBuildings = 5;
    
    [SerializedDictionary("ColonyStatType", "ColonyStat")]
    public SerializedDictionary<ColonyStatType, Stat> stats;
    
    
    [Space(10), Header("Unit Settings")]
    public int unitsAmount;
    public List<GameObject> unitPool;
    [Tooltip("Delay in seconds")] private float spawnDelay;
    
    
    [Space(10), Header("Building pools")]
    public List<BuildingData> resourcePool; // make them all weighted collections?
    public List<BuildingData> barracksPool;
    public List<BuildingData> storagePool;
    public List<BuildingData> defensePool;
    
    [Space(5), Header("Expansion")]
    public ExpansionSequence expansionSequence;
    private bool isDay = true;
    
    [ContextMenu("Add stat")]
    public void AddStatTest()
    {
        stats.Add(maxBuildings, new Stat(8));
        stats.Add(maxDefenses, new Stat(3));
        stats.Add(maxUnits, new Stat(5));
        stats.Add(maxUnitsLevel, new Stat(1));
        stats.Add(unitsSpawnSpeed, new Stat(30));
    }
    
    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        // foreach(ColonyStatType stat in (ColonyStatType[]) Enum.GetValues(typeof(ColonyStatType))){
        //     stats[stat] = new Stat(stats[stat].Value);
        // }
        //stats = new SerializedDictionary<ColonyStatType, Stat>();
        //stats[maxBuildings] = new(8);
        //stats[maxDefenses] = new(3);
        //stats[maxUnits] = new(5);
        //stats[maxUnitsLevel] = new(1);
        //stats[unitsSpawnSpeed] = new(30);
        
        spawnDelay = stats[unitsSpawnSpeed].Value;
        
        expansionSequence = ScriptableObject.Instantiate<ExpansionSequence>(expansionSequence);
        expansionSequence.Init(GameController.timeManager, this);
        
        EventBus.i.OnSunrise += ()=>{isDay = true;};
        EventBus.i.OnSunset += ()=>{isDay = false;};
        EventBus.i.OnMinuteChange += OnMinuteChange;
        
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
    
    protected virtual void OnMinuteChange()
    {
        expansionSequence.UpdateSequence(isDay);
    }

    void Update()
    {
        if(CanSpawnUnit()){
            HandleUnitSpawn();
        }
    }
    
    protected void HandleUnitSpawn()
    {
        if(spawnDelay > 0){
            spawnDelay -= Time.deltaTime;
        }
        else if(spawnDelay <= 0){
            SpawnRandomUnit();
            
            spawnDelay = stats[unitsSpawnSpeed].Value;
        }
    }
    protected void SpawnRandomUnit()
    {
        if(!CanSpawnUnit()){
            Debug.Log("Max units reached.", gameObject);
            return;
        }
        
        Unit u = null;
        GameObject randUnit = unitPool[Random.Range(0, unitPool.Count)];
        if(buildings.Count > 0){
            Building randBuilding = buildings[Random.Range(0, buildings.Count)];
            u = SpawnUnit(randUnit, randBuilding);
        }
        else{
            u = SpawnUnit(randUnit);
        }
        u.GetComponent<Enemy>().InitEnemy(this);
        u.Init();
    }
    protected bool CanSpawnUnit(){
        return unitsAmount < stats[maxUnits].Value;
    }
    protected Unit SpawnUnit(GameObject prefab, Building building = null)
    {
        Vector2 spawnPos = new Vector2(transform.position.x, transform.position.z);
        if(building != null){
            spawnPos = new Vector2(building.transform.position.x, building.transform.position.z);
        }
        
        Vector3 pointInCircle = GameController.RandomPointInCircleTerrain(spawnPos, 2, 5);
        GameObject go = Instantiate(prefab, pointInCircle, Quaternion.identity);
        Unit u = go.GetComponent<Unit>();
        u.onUnitDeath += OnColonyUnitDeath;
        
        AssignUnitToBuildingAlerts(u); // stupid lambda function, нельзя отписаться никак. кучу памяти жрёт если не задереференсить?
        
        unitsAmount++;
        return u;
    }
    protected void OnColonyUnitDeath(Unit unit)
    {
        unitsAmount--;
        unit.onUnitDeath -= OnColonyUnitDeath;
    }
    protected void AssignUnitToBuildingAlerts(Unit unit) // jesus
    {
        for(int i = 0; i < buildings.Count; i++)
        {
            buildings[i].OnAttacked += b => unit.SetHome(b.transform.position);
        }
        OnAttacked += b => unit.SetHome(b.transform.position);
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
                
                // foreach(var stat in stats)
                // {
                //     Debug.Log($"{stat.Key} = {stat.Value.Value}");
                // }
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
            //Debug.Log($"Applied mod {mod.Key}:{mod.Value.Value}");
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
        EventBus.i.OnMinuteChange -= OnMinuteChange;
        base.Death();
    }
    
    public override BuildingPanel CreatePanel(RectTransform parentContainer)
    {
        return null;
    }
    
    protected override void OnMouseEnter()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach(var stat in stats){
            stringBuilder.Append($"{stat.Key}: {stat.Value.Value} +{stat.Value.Value-stat.Value.baseValue}\n");
        }
        WorldUI.i.buildingHover.SetupForBuilding(this);
        WorldUI.i.buildingHover.ShowForBuilding(this, stringBuilder.ToString());
    }
    protected override void OnMouseExit()
    {
        WorldUI.i.buildingHover.Hide(this);
    }
    
    protected void OnSunrise()
    {
        //Expand();
    }
    
    protected void Expand()
    {
        Vector2Int newCell = ColoniesManager.i.gridManager._ClosestDifferentCell(cellIndex.x, cellIndex.z);
        CaptureCellInstant(newCell.x, newCell.y);
        //ColoniesManager.i.gridManager.cells[newCell.x, newCell.y].Capture(Affiliation.Enemy);
    }
}
