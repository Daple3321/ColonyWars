using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public enum ColonyStatType
{
    maxBuildings = 0,
    maxDefenses = 1,
    maxUnits = 2,
    maxUnitsLevel = 3,
    unitsSpawnSpeed = 4,
    maxBuildingLevel = 5,
    raidSquadMaxUnits = 6,
    raidSquadSpawnSpeed = 7,
    
}

public enum EntityStatType
{
    maxHealth = 0,
    regenSpeed = 1,
    
    runSpeed = 2,
    maxStamina = 3,
    staminaRegenSpeed = 4,
    
    damage = 5,
    attackSpeed = 6,
    critChance = 7,
    attackDistance = 8,
    
    maxCommandEnergy = 9,
    commandEnergyRegenSpeed = 10,
    
}

[System.Serializable]
public class Stat
{
    [SerializeField] public float baseValue;
    
    public float Value {
        get{ 
            if(isDirty || baseValue != lastBaseValue){
                lastBaseValue = baseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }
    }
    
    private bool isDirty = true;
    private float _value;
    private float lastBaseValue = float.MinValue;
    
    [SerializeField] private List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;
    
    public Stat(float baseValue)
    {
        this.baseValue = baseValue;
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }
    
    public void OnModifierChanged()
    {
        isDirty = true;
    }
    
    public void AddModifier(StatModifier mod)
    {
        isDirty = true;
        statModifiers.Add(mod);
        statModifiers.Sort(CompareModifierOrder);
    }
    
    private int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if(a.Order < b.Order)
            return -1;
        else if(a.Order > b.Order)
            return 1;
        return 0;
    }
    
    public bool RemoveModifier(StatModifier mod)
    {
        if(statModifiers.Remove(mod)){
            isDirty = true;
            return true;
        }
        return false;
    }
    
    public bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;
        
        for(int i = statModifiers.Count-1; i>=0; i--)
        {
            if(statModifiers[i].Source == source)
            {
                isDirty = true;
                didRemove = true;
                //Debug.Log($"Removing mod {statModifiers[i].Value} of source: {statModifiers[i].Source}");
                statModifiers.RemoveAt(i);
            }
        }
        
        return didRemove;
    }
    
    private float CalculateFinalValue()
    {
        float finalValue = baseValue;
        float sumPercentAdd = 0;
        
        for(int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];
            
            if(mod.Type == StatModType.Flat)
            {
                finalValue += mod.Value;
            }
            else if(mod.Type == StatModType.PercentMult)
            {
                finalValue *= 1 + mod.Value;
            }
            else if(mod.Type == StatModType.PercentAdd)
            {
                sumPercentAdd += mod.Value;
                if(i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
        }
        
        return (float)Math.Round(finalValue, 4);
    }
    
}
