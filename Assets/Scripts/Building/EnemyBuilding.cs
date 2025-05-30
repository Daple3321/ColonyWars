using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class EnemyBuilding : Building
{
    [SerializedDictionary("Stat To Modifiy","Value To Add")]
    public SerializedDictionary<ColonyStatType, float> flatModifiers;
    [SerializedDictionary("Stat To Modifiy","Percent")]
    public SerializedDictionary<ColonyStatType, float> percentModifiers;
    
    public Dictionary<ColonyStatType, StatModifier> GetModifiers()
    {
        Dictionary<ColonyStatType, StatModifier> finalDict = new Dictionary<ColonyStatType, StatModifier>();
        
        foreach(var mod in flatModifiers)
        {
            //finalDict[mod.Key] = flatModifiers[mod.Key];
            finalDict.Add(mod.Key, new StatModifier(mod.Value, StatModType.Flat, this));
        }
        foreach(var mod in percentModifiers)
        {
            //finalDict[mod.Key] = flatModifiers[mod.Key];
            finalDict.Add(mod.Key, new StatModifier(mod.Value, StatModType.PercentMult, this));
        }
        
        return finalDict;
    }
    
    
    public override void Death()
    {
        
        
        
        base.Death();
    }
}
