using System;
using System.Collections.Generic;
using UnityEngine;
using static EntityStatType;

[System.Serializable]
public class Squad
{
    public List<Unit> units;
    public int maxUnits;
    
    public SquadStats stats;
    
    public GameObject squadOwner;
    public Transform followTarget;
    public Action<Squad> onSquadUpdate;

    public Squad(int maxUnits, GameObject owner = null)
    {
        this.maxUnits = maxUnits;
        this.squadOwner = owner;
        units = new List<Unit>();
        stats = SquadStats.GetEmptyStats();
    }
    
    public bool IsEmpty(){ return units.Count <= 0; }
    public bool IsFull() { return units.Count >= maxUnits; }

    public void FollowOrder(Transform followTarget)
    {
        if (units.Count <= 0)
            return;

        // this.followTarget = followTarget;

        // foreach (Unit unit in units)
        // {
        //     unit.followTarget = followTarget;
        //     unit.StartFollowing();
        // }
        
        this.followTarget = squadOwner.transform;
        foreach (Unit unit in units)
        {
            unit.followTarget = squadOwner.transform;
            unit.StartFollowing();
        }
    }
    public void FollowOrder(Unit targetUnit)
    {
        if (units.Count <= 0)
            return;
        if(!units.Contains(targetUnit)){
            Debug.LogError($"Can't order to {targetUnit.unitName}. Not in this squad");
            return;
        }
        
        targetUnit.followTarget = squadOwner.transform;
        targetUnit.StartFollowing();
    }
    public void MoveOrder(Vector3 orderPos)
    {
        if (units.Count <= 0)
            return;

        foreach (Unit unit in units)
        {
            Vector3 offset = CalculateOffsetForUnit(units.IndexOf(unit), 1.8f);
            
            unit.SetHome(orderPos + offset);
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
            Debug.LogWarning("Unit already in THIS squad!");
            return false;
        }
        if(unit.InSquad()){
            Debug.LogWarning($"Unit already in ANOTHER squad! Squad owner: {unit.squad.squadOwner}");
            return false;
        }
        if(unit.gameObject == squadOwner){
            Debug.LogWarning("Unit is THE OWNER of this squad!");
            return false;
        }

        units.Add(unit);
        unit.RegisterToSquad(this);
        unit.onUnitDeath += RemoveUnit;
        
        UpdateStats();
        onSquadUpdate?.Invoke(this);
        
        return true;
    }
    
    public void OutlineSquad()
    {
        foreach(Unit u in units)
        {
            Outline o = u.characterObject.AddComponent<Outline>();
            if(o != null){
                o.OutlineColor = GameAssets.colors.availableColor;
                o.OutlineWidth = 4.5f;
            }
        }
    }
    public void RemoveOutlines(){
        foreach(Unit u in units)
        {
            if(u.characterObject.TryGetComponent(out Outline o))
            {
                GameObject.Destroy(o);
            }
        }
    }
    
    public Vector3 GetCenterPosition()
    {
        Vector3 meanPos = Vector3.zero;
        foreach(Unit unit in units)
        {
            meanPos += unit.transform.position;
        }
        meanPos /= units.Count;
        
        return meanPos;
    }
    
    public Vector3 CalculateOffsetForUnit(int unitIndex, float separation)
    {
        Vector3 offset = Vector3.zero;
        float row_offset = unitIndex / Mathf.Sqrt(units.Count) * separation;
        float col_offset = unitIndex % Mathf.RoundToInt(Mathf.Sqrt(units.Count)) * separation;
        offset.x = col_offset;
        offset.z = row_offset;
        
        return offset;
    }
    
    public void UpdateStats()
    {
        float meanDmg = 0;
        foreach(Unit unit in units)
        {
            if(unit.combat != null){
                meanDmg += unit.combat.stats[damage].Value;
            }
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