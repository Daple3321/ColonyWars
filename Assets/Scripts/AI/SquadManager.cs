using System;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    public Squad squad;
    
    public float squadCallDistance;
    public LayerMask unitsMask;
    
    public int currentUnits;
    public int maxUnits;
    
    public SquadPanel squadUI;
    public SquadAssemblePanel squadAssembleUI;
    public float searchDelay;
    private float _searchDelay;
    public bool searchingForUnits = false;
    public List<Unit> nearbyUnits = new List<Unit>();
    
    public Action<Squad> onSquadUpdate;

    void Awake(){
        enabled = false;
    }

    public void Init(){
        squad = new Squad(maxUnits);
        squadUI = GameController.i.playerSquadPanel;
        squadAssembleUI = GameController.i.squadAssemblePanel;
        squadUI.Init(squad);
        squadAssembleUI.Init(this);
        SwitchAssemblePanel();
        onSquadUpdate += squadUI.OnSquadUpdate; 
        _searchDelay = searchDelay;
        
        enabled = true;
    }
    
    void Update()
    {
        if(_searchDelay > 0 && searchingForUnits){
            _searchDelay -= Time.deltaTime;
        }
        else if(_searchDelay <= 0 && searchingForUnits){
            SearchForUnits();
            squadAssembleUI.UpdateUI(nearbyUnits);
            _searchDelay = searchDelay;
        }
    }
    
    public void SwitchAssemblePanel()
    {
        if(squadAssembleUI.isActiveAndEnabled){
            searchingForUnits = false;
            squadAssembleUI.gameObject.SetActive(false);
        }
        else{
            searchingForUnits = true;
            //squadAssembleUI.ClearUI();
            squadAssembleUI.gameObject.SetActive(true);
        }
    }
    public bool SearchForUnits()
    {
        var hitColliders = Physics.OverlapSphere(transform.position, squadCallDistance, unitsMask);
        nearbyUnits.Clear();
        if (hitColliders.Length > 0)
        {
            foreach (Collider col in hitColliders)
            {
                Unit hitUnit;
                if (col.TryGetComponent<Unit>(out hitUnit))
                {
                    nearbyUnits.Add(hitUnit);
                }
            }
            return true;
        }
        else{
            return false;
        }
    }
    
    public void FollowOrder(){
        squad.FollowOrder(transform);
    }
    public void HomePosOrder(Vector3 orderPos){
        squad.MoveOrder(orderPos);
    }
    public void UnitOrder(Vector3 orderPos, Unit targetUnit){
        squad.MoveOrder(orderPos, targetUnit);
    }
    
    public void AddUnit(Unit unit)
    {
        squad.TryAddUnit(unit);
        onSquadUpdate?.Invoke(squad);
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
            //squad.FollowOrder(transform);
            
            onSquadUpdate?.Invoke(squad);
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
        if (!units.Contains(unitToRemove)){
            //Debug.LogWarning($"{unitToRemove.name} not found in squad.");
            return;
        }
        else{
            //Debug.Log("Removing unit");
            unitToRemove.UnregisterFromSquad();
            units.Remove(unitToRemove);
        }
    }
    public void RemoveAllUnits()
    {
        foreach (Unit unit in units)
        {
            unit.UnregisterFromSquad();
        }
        units.Clear();
    }

    public void TryAddUnit(Unit unit)
    {
        if (units.Count >= maxUnits)
            return;
        if (units.Contains(unit)){
            Debug.LogWarning("Unit already in squad!");
            return;
        }
        // if(!unit.InSquad()){
            
        // }

        units.Add(unit);
        unit.RegisterToSquad(this);
        unit.onUnitDeath += RemoveUnit;
    }
}

public enum CommandType : byte
{
    NONE,
    FOLLOW,
    HOMEPOS,
    CREATE_SQUAD,
}