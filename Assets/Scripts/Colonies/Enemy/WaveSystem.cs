using System;
using System.Collections.Generic;
using MackySoft.Choice;
using UnityEngine;
using Random = UnityEngine.Random;
using static EntityStatType;
using static ColonyStatType;
using IngameDebugConsole;

public class WaveSystem : MonoBehaviour
{
    void Awake(){enabled = false;}

    public List<WaveContainer> waveContainers;
    public int containerIndex = 0;
    public int minutesLeftToNextContainer = 0;
    
    private EnemyColony owner;
    public void Init(EnemyColony enemyColony)
    {
        owner = enemyColony;
        minutesLeftToNextContainer = waveContainers[containerIndex].durationMinutes;
        
        DebugLogConsole.AddCommandInstance( "wave", "Spawns current raid wave", nameof(SpawnCurrentWave), this );
        DebugLogConsole.AddCommandInstance( "rTest", "Spawns one raid unit", nameof(TestRaidUnit), this );
        
        enabled = true;
    }
    private void TestRaidUnit()
    {
        Building randBuilding = owner.buildings[Random.Range(0, owner.buildings.Count)];
        
        int waveIdx = Random.Range(0, waveContainers[containerIndex].waves.Count);
        Wave wave = waveContainers[containerIndex].waves[waveIdx];
        
        int unitsAmount = Random.Range(wave.unitAmountRange.x, wave.unitAmountRange.y);
        int amountBonus = CalculateUnitAmountBonusForWave(wave);
        unitsAmount += amountBonus;
        int unitLevel = CalculateUnitLevelForWave(wave);
        
        IWeightedSelector<KeyValuePair<UnitData, float>> selector;
        selector = wave.unitPool.ToWeightedSelector(x => x.Value);
        UnitData selectedUnit;
        selectedUnit = selector.SelectItemWithUnityRandom().Key;
        
        Unit spawnedUnit = owner.SpawnRaidUnit(selectedUnit, randBuilding);
                
        spawnedUnit.stateMachine.startState = GameAssets.idleState;
        if(spawnedUnit.TryGetComponent(out Enemy enemy)){
            enemy.InitEnemy(owner);
        }
        spawnedUnit.Init();
        
        spawnedUnit.ChangeLevel(unitLevel);
    }
    
    public void UpdateWaveContainer()
    {
        if(containerIndex >= waveContainers.Count-1) return;
        
        if(minutesLeftToNextContainer > 0){
            minutesLeftToNextContainer--;
        }
        else if(minutesLeftToNextContainer <= 0 && containerIndex < waveContainers.Count-1){
            containerIndex++;
        }
    }
    
    public Unit[] SpawnCurrentWave()
    {
        Building randBuilding = owner.buildings[Random.Range(0, owner.buildings.Count)];
        
        int waveIdx = Random.Range(0, waveContainers[containerIndex].waves.Count);
        Wave wave = waveContainers[containerIndex].waves[waveIdx];
        
        int unitsAmount = Random.Range(wave.unitAmountRange.x, wave.unitAmountRange.y);
        int amountBonus = CalculateUnitAmountBonusForWave(wave);
        unitsAmount += amountBonus;
        int unitLevel = CalculateUnitLevelForWave(wave);
        
        Unit[] units = new Unit[unitsAmount];
        IWeightedSelector<KeyValuePair<UnitData, float>> selector;
        selector = wave.unitPool.ToWeightedSelector(x => x.Value);
        for(int i = 0; i < unitsAmount; i++)
        {
            UnitData selectedUnit;
        
            selectedUnit = selector.SelectItemWithUnityRandom().Key;
            
            Unit spawnedUnit = owner.SpawnRaidUnit(selectedUnit, randBuilding);
            units[i] = spawnedUnit;
            
            spawnedUnit.stateMachine.startState = GameAssets.idleState;
            if(spawnedUnit.TryGetComponent(out Enemy enemy)){
                enemy.InitEnemy(owner);
            }
            spawnedUnit.Init();
            
            spawnedUnit.ChangeLevel(unitLevel);
        }
        
        return units;
    }
    
    public int CalculateUnitAmountBonusForWave(Wave wave)
    {
        return (int)Mathf.Lerp(0, owner.stats[raidSquadMaxUnits].Value, wave.colonyInfluence_UnitAmount);
    }
    public int CalculateUnitLevelForWave(Wave wave)
    {
        return (int)Mathf.Lerp(1, owner.stats[maxUnitsLevel].Value, wave.colonyInfluence_UnitLevel);
    }
    
}
