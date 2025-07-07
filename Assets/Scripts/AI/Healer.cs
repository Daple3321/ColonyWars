using System.Collections.Generic;
using UnityEngine;

public class Healer : Unit
{
    [Space(7f), Header("Healer settings")]
    public Zone triggerZone;
    public LayerMask healMask;
    public float healRadius = 3f;
    public float healAmount = 5f;
    public float healRate = 2f; // seconds
    
    
    public UnitState idleState;
    public UnitState retreatState;
    public UnitState followState;

    public override void Init()
    {
        base.Init();
        
        healTargets = new();
        _healDelay = healRate;
        
        triggerZone = ZoneFactory.CreateTriggerZone(transform.position, GameAssets.colors.availableColor, ZoneShape.Cylinder, healRadius, 6f);
        triggerZone.transform.SetParent(transform);
        triggerZone.transform.localPosition = Vector3.zero;
        // triggerZone.OnZoneEnter += OnZoneEnter;
        // triggerZone.OnZoneExit += OnZoneExit;

        combat.Init(this);
        combat.currentAttack = new RangedAttack(this, combat.attackData, affiliation);
        
        ConfigureStates();
        Invoke(nameof(StartStates), Random.Range(0.1f, 3f));
    }
    private void StartStates()
    {
        stateMachine.ChangeState(idleState);
    }
    
    public override void ConfigureStates()
    {
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
        
        retreatState.Init(idleState, retreatState);
        idleState.Init(retreatState, retreatState);
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

    protected override void Update(){
        base.Update();
        HandleHeal();
    }
    
    public List<Unit> healTargets;
    private float _healDelay;
    protected void HandleHeal()
    {
        if(healTargets.Count <= 0) return;
        
        if(_healDelay > 0){
            _healDelay -= Time.deltaTime;
        }
        else{
            HealAllTargets();
            _healDelay = healRate;
        }
    }
    protected void HealAllTargets()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, healRadius, healMask);
        if(hits.Length > 0)
        {
            foreach(Collider target in hits)
            {
                target.GetComponent<Unit>().AddHealth(healAmount);
            }
        }
        
        // foreach(Unit target in healTargets)
        // {
        //     target.AddHealth(healAmount);
        // }
    }
    // protected void OnZoneEnter(GameObject go)
    // {
    //     if(go.TryGetComponent(out Unit unit) && unit.affiliation == affiliation)
    //     {
    //         if(!healTargets.Contains(unit)){
    //             healTargets.Add(unit);
    //         }
    //     }
    // }
    // protected void OnZoneExit(GameObject go)
    // {
    //     if(go.TryGetComponent(out Unit unit) && unit.affiliation == affiliation)
    //     {
    //         if(healTargets.Contains(unit)){
    //             healTargets.Remove(unit);
    //         }
    //     }
    // }

    public override void Death()
    {
        //onUnitDeath?.Invoke(this);
        //Destroy(gameObject);
        base.Death();
    }
}
