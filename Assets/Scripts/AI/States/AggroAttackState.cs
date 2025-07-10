using UnityEngine;
using static EntityStatType;

[CreateAssetMenu(fileName = "New Aggro attack State", menuName = "AI/States/Aggro attack state")]
public class AggroAttackState : UnitState
{
    public Enemy enemyOwner;
    
    public UnitState OnAggrEnd;
    public UnitState OnNoTarget;
    
    public override void Enter(StateMachine stateMachine)
    {
        if(stateMachine.owner is Enemy e){
            enemyOwner = e;
        }
    }
    public override void Update(StateMachine stateMachine)
    {
        if (stateMachine.owner.HasTarget() && stateMachine.owner.DistanceToTarget() <= stateMachine.owner.combat.stats[attackDistance].Value)
        {
            stateMachine.owner.combat.HandleAttacking();
            stateMachine.owner.movement.RotateTo(stateMachine.owner.attackTarget.position);
        }
        else if(stateMachine.owner.combat.isAttacking && 
        (!stateMachine.owner.HasTarget() || stateMachine.owner.DistanceToTarget() > stateMachine.owner.combat.stats[attackDistance].Value)){
            //Debug.Log("Attack canceled");
            stateMachine.owner.combat.CancelAttackDelayed();
        }
        
        if(!stateMachine.owner.stun.IsStunned()){
            stateMachine.owner.MoveToAttackTarget();
        }
        
        if(!enemyOwner.aggrActive){ // если закончился аггр
            stateMachine.ChangeState(OnAggrEnd);
        }
        if(!stateMachine.owner.HasTarget()){ // если таргета больше нет
            stateMachine.ChangeState(OnNoTarget);
        }
    }

    public override void Exit()
    {

    }
}
