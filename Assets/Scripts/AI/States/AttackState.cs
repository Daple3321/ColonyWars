using UnityEngine;

public class AttackState : UnitState
{
    private float enemyCheckDelay;
    
    public override void Enter()
    {
        this.enemyCheckDelay = owner.enemyCheckDelay;
    }
    public override void Update()
    {
        if (owner.HasTarget() && owner.DistanceToTarget() <= owner.attackDistance)
        {
            owner.HandleAttacking();
        }
        else if(owner.HasTarget())
        {
            owner.MoveToCurrentTarget();
        }
        
        if (owner.DistanceToHome() > owner.homeRadius)
        {
            stateMachine.ChangeState(previousState);
        }

        if (enemyCheckDelay > 0)
        {
            enemyCheckDelay -= Time.deltaTime;
        }
        else
        {
            enemyCheckDelay = owner.enemyCheckDelay;
            if (!owner.CheckForEnemies())
            {
                stateMachine.ChangeState(previousState);
            }
        }
    }

    public override void Exit()
    {

    }
}
