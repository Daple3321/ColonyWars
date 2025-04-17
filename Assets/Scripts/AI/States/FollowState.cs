using UnityEngine;

public class FollowState : UnitState
{
    public float stopDistance = 2f;
    
    public override void Enter()
    {

    }
    public override void Update()
    {
        if (owner.followTarget != null && Vector3.Distance(owner.transform.position, owner.followTarget.position) > stopDistance)
        {
            owner.FollowTarget();
        }
        
        if(owner.followTarget == null){
            stateMachine.ChangeState(nextState);
        }
    }

    public override void Exit()
    {
        
    }
}
