using UnityEngine;
using static EntityStatType;

public class AttackState : UnitState
{
    private float enemyCheckDelay;
    
    public override void Enter()
    {
        this.enemyCheckDelay = owner.enemyCheckDelay;
    }
    public override void Update()
    {
        if (owner.HasTarget() && owner.DistanceToTarget() <= owner.combat.stats[attackDistance].Value)
        {
            owner.combat.HandleAttacking();
            owner.RotateTo(owner.attackTarget.position);
        }
        owner.MoveToAttackTarget();
        // if(owner.HasTarget() && owner.DistanceToTarget() > owner.attackDistance)
        // {
        //     owner.MoveToAttackTarget();
        // }
        
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
