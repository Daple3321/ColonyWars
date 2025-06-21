using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ColonyStatType;
using Random = UnityEngine.Random;

[System.Serializable]
public class RaidSquad
{
    public Squad squad;
    public List<Unit> unitsOnMission;
    
    [Space(5), Header("Raid")]
    public Cell targetCell;
    Vector3Int targetCellIndex = new Vector3Int(0, 0, 0);
    public List<Building> targetBuildings;
    //public Building buildingTarget;
    public bool hasTarget = false;
    public bool isCompleted = false;
    public float maxRaidTime = 128f;
    public Action<Cell> OnCellCleared;
    
    public List<UnitData> unitPool;
    
    private float _spawnDelay;
    private EnemyColony colony;
    
    public void Init(EnemyColony colony)
    {
        this.colony = colony;
        squad = new Squad((int)colony.stats[raidSquadMaxUnits].Value);
        targetBuildings = new List<Building>();
        unitsOnMission = new List<Unit>();
        _spawnDelay = colony.stats[raidSquadSpawnSpeed].Value;
        
        _raidTime = maxRaidTime;
    }
    
    public void HandleSpawning(float spawnDelay)
    {
        if(_spawnDelay > 0){
            _spawnDelay -= Time.deltaTime;
        }
        else if(_spawnDelay <= 0 && !squad.IsFull())
        {
            Unit u = colony.SpawnRaidUnit(unitPool[Random.Range(0, unitPool.Count)]);
            u.GetComponent<Enemy>().InitEnemy(colony);
            u.Init();
            squad.TryAddUnit(u);
            
            _spawnDelay = spawnDelay;
        }
    }
    private void OnUnitSentToMission(Unit u)
    {
        if(unitsOnMission.Contains(u)) return;
        
        unitsOnMission.Add(u);
    }
    private void OnUnitOnMissionDeath(Unit u){
        if(unitsOnMission.Contains(u)){
            unitsOnMission.Remove(u);
        }
    }
    public bool HasUnitsOnMission(){
        return unitsOnMission.Count > 0;
    }
    public bool IsCaptureSuccesful(){
        if(CanCaptureTargetCell() && HasUnitsOnMission()){
            return true;
        }
        else if(!CanCaptureTargetCell() || !HasUnitsOnMission()){
            return false;
        }
        else{
            return false;
        }
    }
    
    public void StartRaid(Cell targetCell)
    {
        this.targetCell = targetCell;
        targetCellIndex = ColoniesManager.i.grid.WorldToCell(targetCell.worldPosition);
        targetBuildings.Clear();
        
        Collider[] cols = ColoniesManager.i.gridManager.GetCellBuildings(targetCellIndex.x, targetCellIndex.z, Affiliation.Player);
        if(cols.Length > 0)
        {
            foreach (Collider col in cols){
                Building b = col.GetComponent<Building>();
                b.OnBuildingDestroyed += OnTargetBuildingDestroyed;
                targetBuildings.Add(b);
            }
            
            squad.MoveOrder(targetBuildings[0].transform.position);
            foreach(Unit u in squad.units){
                OnUnitSentToMission(u);
                u.onUnitDeath += OnUnitOnMissionDeath;
            }
        }
        
        hasTarget = true;
        isCompleted = false;
        _raidTime = 128f;
    }
    
    private float _raidTime;
    public void HandleRaid()
    {
        if(!hasTarget) return;
        
        if(_raidTime > 0){
            _raidTime -= Time.deltaTime;
        }
    }
    
    public void OnTargetBuildingDestroyed(Building building)
    {
        building.OnBuildingDestroyed -= OnTargetBuildingDestroyed;
        targetBuildings.Remove(building);
        
        if(targetBuildings.Count != 0 ){
            squad.MoveOrder(targetBuildings[0].transform.position);
        }
        else
        {
            OnCellCleared?.Invoke(targetCell);
            hasTarget = false;
            isCompleted = true;
            targetCell = null;
            targetBuildings.Clear();
            
            Debug.Log($"{colony.gameObject.name} has cleared a cell!", colony.gameObject);
        }
    }
    
    public bool CanCaptureTargetCell(){
        return targetBuildings.Count <= 0;
    }
}
