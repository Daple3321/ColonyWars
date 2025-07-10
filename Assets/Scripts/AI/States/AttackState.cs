using UnityEngine;
using static EntityStatType;

[CreateAssetMenu(fileName = "New Attack State", menuName = "AI/States/Attack State")]
public class AttackState : UnitState
{
    private float enemyCheckDelay;
    
    public UnitState OnHomeRadiusExit;
    public UnitState OnNoEnemiesFound;
    
    public override void Enter(StateMachine stateMachine)
    {
        this.enemyCheckDelay = stateMachine.owner.enemyCheckDelay;
    }
    public override void Update(StateMachine stateMachine)
    {
        if (stateMachine.owner.HasTarget() && 
        stateMachine.owner.DistanceToTarget() <= stateMachine.owner.combat.stats[attackDistance].Value && 
        !stateMachine.owner.stun.IsStunned())
        {
            stateMachine.owner.combat.HandleAttacking();
            stateMachine.owner.movement.RotateTo(stateMachine.owner.attackTarget.position);
        }
        else if(stateMachine.owner.combat.isAttacking && 
        (!stateMachine.owner.HasTarget() || stateMachine.owner.DistanceToTarget() > stateMachine.owner.combat.stats[attackDistance].Value)){
            stateMachine.owner.combat.CancelAttackDelayed();
        }
        
        if(!stateMachine.owner.stun.IsStunned()){
            stateMachine.owner.MoveToAttackTarget();
        }
        
        if (stateMachine.owner.DistanceToHome() > stateMachine.owner.homeRadius)
        {
            stateMachine.ChangeState(OnHomeRadiusExit);
        }

        if (enemyCheckDelay > 0)
        {
            enemyCheckDelay -= Time.deltaTime;
        }
        else
        {
            enemyCheckDelay = stateMachine.owner.enemyCheckDelay;
            if (!stateMachine.owner.CheckForEnemies())
            {
                stateMachine.ChangeState(OnNoEnemiesFound);
            }
        }
    }

    public override void Exit()
    {

    }
}
