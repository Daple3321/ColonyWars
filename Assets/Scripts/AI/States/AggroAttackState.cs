using UnityEngine;

public class AggroAttackState : UnitState
{
    public Enemy enemyOwner;
    
    public override void Enter()
    {
        
    }
    public override void Update()
    {
        if (owner.HasTarget() && owner.DistanceToTarget() <= owner.attackDistance)
        {
            owner.HandleAttacking();
            owner.RotateTo(owner.attackTarget.position);
        }
        owner.MoveToAttackTarget();
        
        if(!enemyOwner.aggrActive){
            stateMachine.ChangeState(nextState);
        }
    }

    public override void Exit()
    {

    }
}
