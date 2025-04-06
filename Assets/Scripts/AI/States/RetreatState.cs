using UnityEngine;

public class RetreatState : UnitState
{
    private float enemyCheckDelay;
    
    public override void Enter()
    {
        this.enemyCheckDelay = owner.enemyCheckDelay;
    }
    public override void Update()
    {
        owner.MoveToHome();

        if (owner.DistanceToHome() <= 1.5f)
        {
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
