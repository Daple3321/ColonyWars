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

        if (owner.DistanceToTarget() <= owner.attackDistance)
        {
            //stateMachine.ChangeState(nextState);
            //Debug.Log($"{owner.name} attacked!");
        }
        else
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
