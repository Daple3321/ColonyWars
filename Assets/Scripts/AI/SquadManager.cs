using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    public Squad squad;
    
    public float squadCallDistance;
    public LayerMask unitsMask;
    
    public int currentUnits;
    public int maxUnits;

    void Awake()
    {
        enabled = false;
    }

    public void Init()
    {
        squad = new Squad(maxUnits);
        enabled = true;
    }

    public void SquadOrder(Vector3 orderPos)
    {
        squad.MoveOrder(orderPos);
    }
    public void UnitOrder(Vector3 orderPos, Unit targetUnit)
    {
        squad.MoveOrder(orderPos, targetUnit);
    }

    public bool TryAssembleSquad()
    {
        var hitColliders = Physics.OverlapSphere(transform.position, squadCallDistance, unitsMask);
        if (hitColliders.Length > 0)
        {
            foreach (Collider col in hitColliders)
            {
                Unit hitUnit;
                if (col.TryGetComponent<Unit>(out hitUnit))
                {
                    //Debug.Log($"Hit unit: {hitUnit.name}");
                    squad.TryAddUnit(hitUnit);
                }
            }
            squad.FollowOrder(transform);
            return true;
        }
        else
        {
            Debug.Log("No units in radius to create a squad.");

            return false;
        }
    }
}

[System.Serializable]
public class Squad
{
    public List<Unit> units;
    public int maxUnits;
    public Transform followTarget;

    public Squad(int maxUnits)
    {
        this.maxUnits = maxUnits;
        units = new List<Unit>();
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
        }
        units.Clear();
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
    }

    public void RemoveUnit(Unit unitToRemove)
    {
        if (!units.Contains(unitToRemove))
        {
            Debug.Log($"{unitToRemove.name} not found in squad.");
            return;
        }
        else
        {
            Debug.Log("Removing unit");
            units.Remove(unitToRemove);
        }
    }

    public void TryAddUnit(Unit unit)
    {
        if (units.Count >= maxUnits)
            return;
        if (units.Contains(unit))
        {
            Debug.Log("Unit already in squad!");
            return;
        }

        units.Add(unit);
        unit.onUnitDeath += RemoveUnit;
    }
}