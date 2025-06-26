using UnityEngine;

[System.Serializable]
public class IdleState : UnitState
{
    private float enemyCheckDelay;

    public override void Enter()
    {
        this.enemyCheckDelay = owner.enemyCheckDelay;
    }
    public override void Update()
    {
        owner.movement.UpdateAnimationParams();
        
        if (enemyCheckDelay > 0)
        {
            enemyCheckDelay -= Time.deltaTime;
        }
        else
        {
            enemyCheckDelay = owner.enemyCheckDelay;
            if (owner.CheckForEnemies()) // нашли врага в радиусе
            {
                stateMachine.ChangeState(nextState);
            }
        }

        if (owner.DistanceToHome() > 1.5f) // если далеко от дома
        {
            base.Back();
        }
    }

    public override void Exit()
    {
        
    }

}
