using UnityEngine;

public class AggroAttackState : UnitState
{
    public Enemy enemyOwner;
    
    public override void Enter()
    {
        
    }
    public override void Update()
    {
        if (owner.HasTarget() && owner.DistanceToTarget() <= owner.combat.attackDistance)
        {
            owner.combat.HandleAttacking();
            owner.RotateTo(owner.attackTarget.position);
        }
        owner.MoveToAttackTarget();
        
        if(!enemyOwner.aggrActive){ // если закончился аггр
            stateMachine.ChangeState(nextState);
        }
        if(!owner.HasTarget()){ // если таргета больше нет
            stateMachine.ChangeState(nextState);
        }
    }

    public override void Exit()
    {

    }
}
