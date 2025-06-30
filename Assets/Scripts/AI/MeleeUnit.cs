using UnityEngine;

public class MeleeUnit : Unit
{
    public UnitState idleState;
    public UnitState retreatState;
    public UnitState attackState;
    public UnitState followState;

    public override void Init()
    {
        base.Init();
        
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
        //onUnitDeath?.Invoke(this);
        //Destroy(gameObject);
    }
}
