using System.Collections;
using UnityEngine;

public class Enemy : Unit
{
    [Header("States")]
    public UnitState idleState;
    public UnitState patrolState;
    public UnitState retreatState;
    public UnitState attackState;
    public UnitState followState;
    public UnitState aggroState;
    
    [Space(8), Header("Aggro settings")]
    [Tooltip("In seconds")] public float aggrTime = 25f;
    public bool aggrActive = false;
    public GameObject aggrTarget = null; 
    
    public EnemyColony parentColony;
    public override void Init()
    {
        base.Init();

        combat.Init(this);
        combat.currentAttack = new RangedAttack(this, combat.attackData, affiliation);
        
        ConfigureStates();
        
        Invoke(nameof(StartStates), Random.Range(0.1f, 3f));
    }
    public virtual void InitEnemy(EnemyColony parentColony = null)
    {
        this.parentColony = parentColony;
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
        patrolState = new PatrolState
        {
            stateMachine = stateMachine,
            owner = this,
            parentColony = this.parentColony,
            waitTime = 15f,
        };
        aggroState = new AggroAttackState
        {
            enemyOwner = this,
            stateMachine = stateMachine,
            owner = this,
        };
        
        attackState.Init(idleState, retreatState);
        retreatState.Init(patrolState, attackState);
        idleState.Init(attackState, retreatState);
        followState.Init(idleState, retreatState);
        patrolState.Init(attackState, retreatState);
        aggroState.Init(idleState, retreatState);
        //stateMachine.ChangeState(idleState);
    }
    private void StartStates()
    {
        stateMachine.ChangeState(patrolState);
    }
    public override void TakeDamage(float damage, GameObject source = null, Vector3 knockback = new Vector3())
    {
        base.TakeDamage(damage, source, knockback);
        
        if(!aggrActive){
            StartCoroutine(StartAggr(source));
        }
        // if(source != null) // enemy dealt damage
        // {
        // }
        // else{ // player dealt damage
            
        // }
    }
    protected virtual IEnumerator StartAggr(GameObject target)
    {
        attackTarget = target.transform;
        aggrTarget = target;
        stateMachine.ChangeState(aggroState);
        
        aggrActive = true;
        float progress = aggrTime;
        while(progress > 0)
        {
            progress -= Time.deltaTime;
            yield return null;
        }
        
        aggrTarget = null;
        aggrActive = false;
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
        onUnitDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
