using UnityEngine;

public class RaiderEnemy : Enemy
{
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
        aggroState = new AggroAttackState
        {
            enemyOwner = this,
            stateMachine = stateMachine,
            owner = this,
        };
        
        attackState.Init(idleState, retreatState);
        retreatState.Init(idleState, attackState);
        idleState.Init(attackState, retreatState);
        followState.Init(idleState, retreatState);
        //patrolState.Init(attackState, retreatState);
        aggroState.Init(idleState, retreatState);
        //stateMachine.ChangeState(idleState);
    }
    protected override void StartStates()
    {
        stateMachine.ChangeState(idleState);
    }
}
