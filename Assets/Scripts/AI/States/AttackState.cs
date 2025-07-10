using UnityEngine;
using static EntityStatType;

[System.Serializable]
public class AttackState : UnitState
{
    private float enemyCheckDelay;
    
    public override void Enter()
    {
        this.enemyCheckDelay = owner.enemyCheckDelay;
    }
    public override void Update()
    {
        if (owner.HasTarget() && owner.DistanceToTarget() <= owner.combat.stats[attackDistance].Value && !owner.stun.IsStunned())
        {
            owner.combat.HandleAttacking();
            owner.movement.RotateTo(owner.attackTarget.position);
        }
        else if(owner.combat.isAttacking && (!owner.HasTarget() || owner.DistanceToTarget() > owner.combat.stats[attackDistance].Value)){
            owner.combat.CancelAttackDelayed();
        }
        
        if(!owner.stun.IsStunned()){
            owner.MoveToAttackTarget();
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
