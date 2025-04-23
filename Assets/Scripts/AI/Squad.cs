using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Squad
{
    public List<Unit> units;
    public int maxUnits;
    
    public SquadStats stats;
    
    public Transform followTarget;
    public Action<Squad> onSquadUpdate;

    public Squad(int maxUnits)
    {
        this.maxUnits = maxUnits;
        units = new List<Unit>();
        stats = SquadStats.GetEmptyStats();
    }

    public void FollowOrder(Transform followTarget)
    {
        if (units.Count <= 0)
            return;

        this.followTarget = followTarget;

        foreach (Unit unit in units)
        {
            unit.followTarget = followTarget;
            unit.StartFollowing();
        }
    }
    public void MoveOrder(Vector3 orderPos)
    {
        if (units.Count <= 0)
            return;

        foreach (Unit unit in units)
        {
            unit.SetHome(orderPos);
            unit.StopFollowing();
            unit.followTarget = null;
            //unit.UnregisterFromSquad();
        }
        //RemoveAllUnits();

        //units.Clear();
    }
    public void MoveOrder(Vector3 orderPos, Unit targetUnit)
    {
        if (units.Count <= 0)
            return;
        if (!units.Contains(targetUnit))
            return;

        targetUnit.SetHome(orderPos);
        targetUnit.StopFollowing();
        targetUnit.followTarget = null;
        targetUnit.UnregisterFromSquad();
        RemoveUnit(targetUnit);
        
        UpdateStats();
        onSquadUpdate?.Invoke(this);
    }

    public void CreateSquad(GameObject[] unitsToAdd)
    {
        foreach (GameObject unit in unitsToAdd)
        {
            Unit unitToAdd = unit.GetComponent<Unit>();
            if (units.Count < maxUnits)
            {
                units.Add(unitToAdd);
                unitToAdd.onUnitDeath += RemoveUnit; // зачем
            }
            else
                return;
        }
        
        UpdateStats();
        onSquadUpdate?.Invoke(this);
    }

    public void RemoveUnit(Unit unitToRemove)
    {
        if (!units.Contains(unitToRemove)){
            //Debug.LogWarning($"{unitToRemove.name} not found in squad.");
            return;
        }
        else{
            //Debug.Log("Removing unit");
            unitToRemove.UnregisterFromSquad();
            units.Remove(unitToRemove);
        }
        
        UpdateStats();
        onSquadUpdate?.Invoke(this);
    }
    public void RemoveAllUnits()
    {
        foreach (Unit unit in units)
        {
            unit.UnregisterFromSquad();
        }
        units.Clear();
        
        UpdateStats();
        onSquadUpdate?.Invoke(this);
    }

    public bool TryAddUnit(Unit unit)
    {
        if (units.Count >= maxUnits){
            // TO-DO: Some kind of action callback here
            Debug.LogWarning("Squad max capacity!");
            return false;
        }
        if (units.Contains(unit)){
            Debug.LogWarning("Unit already in squad!");
            return false;
        }
        // if(!unit.InSquad()){
            
        // }

        units.Add(unit);
        unit.RegisterToSquad(this);
        unit.onUnitDeath += RemoveUnit;
        
        UpdateStats();
        onSquadUpdate?.Invoke(this);
        
        return true;
    }
    
    public void UpdateStats()
    {
        float meanDmg = 0;
        foreach(Unit unit in units)
        {
            meanDmg += unit.damage;
        }
        meanDmg /= units.Count;
        
        stats.meanDamage = meanDmg;
    }
}

[System.Serializable]
public struct SquadStats
{
    public float speedMultiplier;
    public float meanDamage;
    
    public static SquadStats GetEmptyStats() => new SquadStats{ 
            speedMultiplier = 1,
            meanDamage = 0,
        };
}