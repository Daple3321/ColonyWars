using UnityEngine;

[CreateAssetMenu(fileName = "New Retreat State", menuName = "AI/States/Home Retreat state")]
public class HomeRetreatState : UnitState
{
    private float enemyCheckDelay;
    
    public UnitState onArrivedState;
    public UnitState onEnemyFoundState;
    
    public override void Enter(StateMachine stateMachine)
    {
        this.enemyCheckDelay = stateMachine.owner.enemyCheckDelay;
    }
    public override void UpdateState(StateMachine stateMachine)
    {
        if(!stateMachine.owner.stun.IsStunned()){
            stateMachine.owner.MoveToHome();
            stateMachine.owner.movement.UpdateAnimationParams();
        }

        if (stateMachine.owner.DistanceToHome() <= stateMachine.owner.targetStopDistance)
        {
            stateMachine.owner.movement.ResetVelocity();
            stateMachine.owner.OnArrivedToHome();
            stateMachine.ChangeState(onArrivedState);
            return;
        }

        if (enemyCheckDelay > 0)
        {
            enemyCheckDelay -= Time.deltaTime;
        }
        else
        {
            enemyCheckDelay = stateMachine.owner.enemyCheckDelay;
            if (stateMachine.owner.CheckForEnemies())
            {
                stateMachine.ChangeState(onEnemyFoundState);
                return;  
            }
        }
    }

    public override void Exit()
    {
        
    }
}
