using System;
using System.Collections.Generic;
using MackySoft.Choice;
using UnityEngine;
using Random = UnityEngine.Random;
using static EntityStatType;
using static ColonyStatType;

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
        
        enabled = true;
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
            
            Unit spawnedUnit = owner.SpawnRaidUnit(selectedUnit);
            units[i] = spawnedUnit;
            
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
