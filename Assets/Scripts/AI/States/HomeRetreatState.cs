using UnityEngine;

public class HomeRetreatState : UnitState
{
    private float enemyCheckDelay;
    
    public override void Enter()
    {
        this.enemyCheckDelay = owner.enemyCheckDelay;
    }
    public override void Update()
    {
        if(!owner.stun.IsStunned()){
            owner.MoveToHome();
            owner.movement.UpdateAnimationParams();
        }

        if (owner.DistanceToHome() <= 1.5f)
        {
            owner.movement.ResetVelocity();
            stateMachine.ChangeState(nextState);
        }

        if (enemyCheckDelay > 0)
        {
            enemyCheckDelay -= Time.deltaTime;
        }
        else
        {
            enemyCheckDelay = owner.enemyCheckDelay;
            if (owner.CheckForEnemies())
            {
                stateMachine.ChangeState(previousState);    
            }
        }
    }

    public override void Exit()
    {
        
    }
}
