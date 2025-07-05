using System.Collections.Generic;
using UnityEngine;

public class Commander : Unit, IClickable
{
    public UnitState idleState;
    public UnitState retreatState;
    public UnitState attackState;
    public UnitState followState;
    
    public Squad ownedSquad;
    public float squadCallDistance;
    public LayerMask unitsMask;
    
    public int currentUnits;
    public int maxUnits;
    
    public float searchDelay;
    private float _searchDelay;
    public bool searchingForUnits = false;
    public List<Unit> nearbyUnits = new List<Unit>();
    private Zone searchZone;

    public override void Init()
    {
        base.Init();
        
        ownedSquad = new Squad(maxUnits, gameObject);
        _searchDelay = searchDelay;
        
        searchZone = ZoneFactory.CreateZone(
            transform.position,
            GameAssets.colors.gatherRadius,
            ZoneShape.Cylinder,
            squadCallDistance);
        searchZone.transform.SetParent(transform, false);
        searchZone.transform.localPosition = Vector3.zero;
        //searchZone.gameObject.SetActive(false);
        
        combat.Init(this);
        combat.currentAttack = new MeleeAttack(this, combat.attackData, affiliation);
        
        ConfigureStates();
        Invoke(nameof(StartStates), Random.Range(0.1f, 3f));
    }
    private void StartStates()
    {
        stateMachine.ChangeState(idleState);
    }
    public override void ConfigureStates()
    {
        attackState = new AttackState
        {
            stateMachine = stateMachine,
            owner = this,
        };
        retreatState = new HomeRetreatState
        {
            stateMachine = stateMachine,
            owner = this,
        };
        idleState = new IdleState
        {
            stateMachine = stateMachine,
            owner = this,
        };
        followState = new FollowState
        {
            stateMachine = stateMachine,
            owner = this,
        };
        
        attackState.Init(idleState, retreatState);
        retreatState.Init(idleState, attackState);
        idleState.Init(attackState, retreatState);
        followState.Init(idleState, retreatState);

        stateMachine.ChangeState(idleState);
    }
    public override void StartFollowing()
    {
        stateMachine.ChangeState(followState);
    }
    public override void StopFollowing()
    {
        stateMachine.ChangeState(idleState);
        followTarget = null;
    }
    public override void Death()
    {
        base.Death();
    }
    
    protected override void Update()
    {
        base.Update();
        
        // if(_searchDelay > 0 && searchingForUnits){
        //     _searchDelay -= Time.deltaTime;
        // }
        // else if(_searchDelay <= 0 && searchingForUnits){
        //     SearchForUnits();
        //     //squadAssembleUI.UpdateUI(nearbyUnits);
        //     _searchDelay = searchDelay;
        // }
        
        // if(InSquad()){
        //     Debug.Log($"IN SQUAD OF ONWER: {squad.squadOwner.name}", squad.squadOwner);
        // }
    }
    
    public void SwitchUnitSearching()
    {
        if(searchingForUnits){
            searchingForUnits = false;
            if(searchZone!=null){
                searchZone.gameObject.SetActive(false);
            }
        }
        else{
            searchingForUnits = true;
            searchZone.gameObject.SetActive(true);
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
        ownedSquad.FollowOrder(transform);
    }
    public void HomePosOrder(Vector3 orderPos){
        ownedSquad.MoveOrder(orderPos);
    }
    public void UnitOrder(Vector3 orderPos, Unit targetUnit){
        ownedSquad.MoveOrder(orderPos, targetUnit);
    }
    public void ClearSquad(){
        ownedSquad.RemoveAllUnits();
    }
    
    public bool AddUnit(Unit unit)
    {
        return ownedSquad.TryAddUnit(unit);
    }
    
    public void TryAssembleSquad()
    {
        var hitColliders = Physics.OverlapSphere(transform.position, squadCallDistance, unitsMask);
        if (hitColliders.Length > 0)
        {
            foreach (Collider col in hitColliders)
            {
                if (col.TryGetComponent(out Unit hitUnit) && hitUnit != this)
                {
                    //Debug.Log($"Hit unit: {hitUnit.name}");
                    ownedSquad.TryAddUnit(hitUnit);
                    Debug.Log($"{hitUnit.unitName} ADDED TO COMMANDERS SQUAD!");
                }
            }
            //squad.FollowOrder(transform);
        }
        else{
            Debug.Log("No units in radius to create a squad.");
        }
    }
    
    public bool HasSquad(){
        return !ownedSquad.IsEmpty();
    }

    public void OnClick(Player caller)
    {
        GameController.p.squadManager.OnSquadSelected(this, ownedSquad);
    }
}
