using System.Collections.Generic;
using UnityEngine;
using static ColonyStatType;
using AYellowpaper.SerializedCollections;
using System;
using Random = UnityEngine.Random;
using System.Text;
using System.Collections;
using MackySoft.Choice;
using System.Linq;

[System.Serializable]
public class EnemyColony : Colony
{
    [Space(10), Header("Stats")]
    public int startingBuildings = 5;
    
    [SerializedDictionary("ColonyStatType", "Colony Stat")]
    public SerializedDictionary<ColonyStatType, Stat> stats; // должно быть colonyStats
    
    // лучше это встроить в сами статы. Cделать опцию чтоб при инициализации добавлялся baseModifier
    [SerializedDictionary("ColonyStatType", "Modifier")]
    public SerializedDictionary<ColonyStatType, StatModifier> baseModifiers; // очень странно
    
    
    [Space(10), Header("Unit Settings")]
    public int unitsAmount;
    [SerializedDictionary("Unit", "Chance to spawn")]
    public SerializedDictionary<UnitData, float> unitPool;
    //public List<UnitData> unitPool;
    [Tooltip("Delay in seconds")] private float spawnDelay;
    
    public RaidSquad raidSquad;
    
    
    [Space(10), Header("Building")]
    public ColonyBuilder builder;
    public BuildingData outpost;
    
    
    [Space(5), Header("Expansion")]
    public ExpansionSequence expansionSequence;
    public GridManager.Direction playerDirection;
    private bool isDay = true;
    
    // [ContextMenu("Add stat")]
    // public void AddStatTest()
    // {
    //     stats.Add(maxBuildings, new Stat(8));
    //     stats.Add(maxDefenses, new Stat(3));
    //     stats.Add(maxUnits, new Stat(5));
    //     stats.Add(maxUnitsLevel, new Stat(1));
    //     stats.Add(unitsSpawnSpeed, new Stat(30));
    // }
    
    private void ApplyInitialModifiers()
    {
        foreach(var mod in baseModifiers)
        {
            stats[mod.Key].AddModifier(mod.Value);
        }
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
        ApplyInitialModifiers();
        
        builder.Init(this);
        
        spawnDelay = stats[unitsSpawnSpeed].Value;
        //raidSquad = new RaidSquad((int)stats[raidSquadMaxUnits].Value, stats[raidSquadSpawnSpeed].Value, this);
        raidSquad.Init(this);
        
        expansionSequence = ScriptableObject.Instantiate<ExpansionSequence>(expansionSequence);
        expansionSequence.Init(GameController.timeManager, this);
        CaptureCellInstant(cellIndex.x, cellIndex.z);
        
        EventBus.i.OnSunrise += ()=>{isDay = true;};
        EventBus.i.OnSunset += ()=>{isDay = false;};
        EventBus.i.OnMinuteChange += OnMinuteChange;
        
        // for(int i = 0; i < startingBuildings; i++)
        // {
        //     BuildingData randBuilding = resourcePool.First().Key;
        //     Vector3 pointInCell = ColoniesManager.i.gridManager.RandomPointInCell(cellIndex.x, cellIndex.z);
            
        //     Building b = GameController.objectGenerator.CreateBuilding_Rules(randBuilding, pointInCell);
        //     if(b != null)
        //     {
        //         b.Init(randBuilding);
        //         StartCoroutine(b.Build());
                
        //         AddBuilding(b);
                
        //         ApplyModifiersToStats(b.GetComponent<EnemyBuilding>().GetModifiers());
        //     }
        // }
        
        EventBus.i.OnSunrise += OnSunrise;
    }
    
    protected override void LevelSystem_OnLevelChanged(object sender, EventArgs e)
    {
        baseModifiers[maxBuildings].Value += 1;
        baseModifiers[maxBuildingLevel].Value += 1;
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
        raidSquad.HandleSpawning(stats[raidSquadSpawnSpeed].Value);
        //raidSquad.HandleRaid();
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
        var selector = unitPool.ToWeightedSelector(x => x.Value);
        
        Unit u = null;
        GameObject randUnit = selector.SelectItemWithUnityRandom().Key.prefab;
        // if(buildings.Count > 0){
        //     Building randBuilding = buildings[Random.Range(0, buildings.Count)];
        //     u = SpawnUnit(randUnit, randBuilding, true);
        // }
        // else{
        // }
        u = SpawnUnit(randUnit, null, true);
        if(u.TryGetComponent(out Enemy enemy)){
            enemy.InitEnemy(this);
        }
        u.Init();
        
        int randLevel = 1;
        if(stats[maxUnitsLevel].Value > 1){
            randLevel = Random.Range((int)stats[maxUnitsLevel].Value-1, (int)stats[maxUnitsLevel].Value);
        }
        u.ChangeLevel(randLevel);
    }
    protected bool CanSpawnUnit(){
        return unitsAmount < stats[maxUnits].Value;
    }
    protected Unit SpawnUnit(GameObject prefab, Building building = null, bool assignToAlerts = false)
    {
        Vector2 spawnPos = new Vector2(transform.position.x, transform.position.z);
        if(building != null){
            spawnPos = new Vector2(building.transform.position.x, building.transform.position.z);
        }
        
        Vector3 pointInCircle = GameController.RandomPointInCircleTerrain(spawnPos, 2, 5);
        GameObject go = Instantiate(prefab, pointInCircle, Quaternion.identity);
        Unit u = go.GetComponent<Unit>();
        u.onUnitDeath += OnColonyUnitDeath;
        u.InitPatrolRoute(this);
        
        if(assignToAlerts){
            AssignUnitToBuildingAlerts(u); // stupid lambda function, нельзя отписаться никак. кучу памяти жрёт если не задереференсить?
        }
        
        unitsAmount++;
        return u;
    }
    public Unit SpawnRaidUnit(UnitData unit, Building building = null)
    {
        Vector2 spawnPos = new Vector2(transform.position.x, transform.position.z);
        if(building != null){
            spawnPos = new Vector2(building.transform.position.x, building.transform.position.z);
        }
        
        Vector3 pointInCircle = GameController.RandomPointInCircleTerrain(spawnPos, 2, 5);
        GameObject go = Instantiate(unit.prefab, pointInCircle, Quaternion.identity);
        Unit u = go.GetComponent<Unit>();
        u.InitPatrolRoute(this);
        //u.onUnitDeath += OnColonyUnitDeath;
        
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
    
    public void PerformAction(ColonyAction action, bool immediate = false)
    {
        HandleAction(action.actionType, immediate);
        
        Debug.Log($"{gameObject.name} Performed {action.actionType}", gameObject);
    }
    
    protected virtual void HandleAction(ColonyActionType actionType, bool immediate = false)
    {
        switch(actionType){
        case ColonyActionType.Expand:
            Expand(immediate);
        break;
        
        case ColonyActionType.Capture:
            StartCoroutine(Capture(Affiliation.Player));
        break;
        
        case ColonyActionType.RaidSquad:
           RaidSquad();
        break;
        
        case ColonyActionType.Resources:
            builder.PerformBuildAction(actionType);
        break;
        
        case ColonyActionType.Storage:
            builder.PerformBuildAction(actionType);
        break;
        
        case ColonyActionType.DefenseTower:
            builder.PerformBuildAction(actionType);
        break;
        
        case ColonyActionType.Wall:
            builder.PerformBuildAction(actionType);
        break;
        
        case ColonyActionType.Barracks:
            builder.PerformBuildAction(actionType);
        break;
        
        }
    }
    
    public void Build(BuildingData building, Vector3 pos)
    {
        if(buildings.Count >= stats[maxBuildings].Value){
            levelSystem.LevelUp();
            Debug.Log($"{gameObject.name} lvl up! Lv.{levelSystem.GetLevel()}", gameObject);
        }
        
        Building b = GameController.objectGenerator.CreateBuilding_Rules(building, pos);
        if(b != null)
        {
            b.Init(building);
            StartCoroutine(b.Build());
            
            AddBuilding(b);
            
            if(b is EnemyBuilding eb){
                ApplyModifiersToStats(eb.GetModifiers());
            }
        }
    }
    public void Build(BuildingData building, Vector3 pos, GridManager.Direction rotDir)
    {
        if(buildings.Count >= stats[maxBuildings].Value){
            levelSystem.LevelUp();
            Debug.Log($"{gameObject.name} lvl up! Lv.{levelSystem.GetLevel()}", gameObject);
        }
        
        Building b = GameController.objectGenerator.CreateBuilding_Rules(building, pos, rotDir);
        if(b != null)
        {
            b.Init(building);
            StartCoroutine(b.Build());
            
            AddBuilding(b);
            
            if(b is EnemyBuilding eb){
                ApplyModifiersToStats(eb.GetModifiers());
            }
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
            stringBuilder.Append($"{stat.Key}: {stat.Value.Value}  |  +{stat.Value.Value-stat.Value.baseValue}\n");
        }
        WorldUI.i.buildingHover.SetupForBuilding(this);
        WorldUI.i.buildingHover.Show(this, stringBuilder.ToString());
    }
    protected override void OnMouseExit()
    {
        WorldUI.i.buildingHover.Hide(this);
    }
    
    protected void OnSunrise()
    {
        //Expand();
    }
    
    protected void Expand(bool immediate = false)
    {
        if(!immediate){
            Vector2Int closestCellIndex = ColoniesManager.i.gridManager.ClosestCell(cellIndex.x, cellIndex.z, Affiliation.None);
            //Vector2Int closestCellIndex = await ColoniesManager.i.gridManager.FindClosestCell(cellIndex.x, cellIndex.z, Affiliation.None);
            
            builder.BuildOutpost(closestCellIndex);
        }
        else{
            Vector2Int closestCellIndex = ColoniesManager.i.gridManager.ClosestCell(cellIndex.x, cellIndex.z, Affiliation.None);
            //Vector2Int closestCellIndex = await ColoniesManager.i.gridManager.FindClosestCell(cellIndex.x, cellIndex.z, Affiliation.None);
            
            CaptureCellInstant(closestCellIndex.x, closestCellIndex.y);

            builder.BuildOutpost(closestCellIndex);
        }
    }
    
    protected void RaidSquad(Vector2Int cellTarget = default)
    {
        if(cellTarget == default){
            Vector2Int closestCellIndex = ColoniesManager.i.gridManager.ClosestCell(cellIndex.x, cellIndex.z, Affiliation.Player);
            //Vector2Int closestCellIndex = await ColoniesManager.i.gridManager.FindClosestCell(cellIndex.x, cellIndex.z, Affiliation.Player);
            
            if(closestCellIndex.x != -1){
                Cell closestCell = ColoniesManager.i.gridManager.GetCell(closestCellIndex.x, closestCellIndex.y);
                raidSquad.StartRaid(closestCell);
            }
        }
        else{
            Cell closestCell = ColoniesManager.i.gridManager.GetCell(cellTarget.x, cellTarget.y);
            raidSquad.StartRaid(closestCell);
        }
    }
    
    
    protected IEnumerator Capture(Affiliation cellAffiliation)
    {
        Vector2Int closestCellIndex = ColoniesManager.i.gridManager.ClosestCell(cellIndex.x, cellIndex.z, cellAffiliation);
        Cell closestCell = ColoniesManager.i.gridManager.GetCell(closestCellIndex.x, closestCellIndex.y);
        RaidSquad(closestCellIndex);
        
        yield return new WaitUntil(() => !raidSquad.HasUnitsOnMission() || raidSquad.CanCaptureTargetCell());
        
        if(raidSquad.IsRaidSuccesful())
        {
            //Vector3 outpostPos = ColoniesManager.i.gridManager.RandomPointInCell(closestCell);
            builder.BuildOutpost(closestCellIndex);
            
            Debug.Log($"Outpost built.", gameObject);
        }
        else{
            raidSquad.squad.MoveOrder(GameController.RandomPointInCircleTerrain(new Vector2(transform.position.x, transform.position.z), 2, 5));
            Debug.Log("Capture failed. No raiders alive or there are player buildings.");
        }
    }
}
