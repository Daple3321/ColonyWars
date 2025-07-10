using UnityEngine;

[System.Serializable]
public class FollowState : UnitState
{
    public float stopDistance = 2f;
    
    public override void Enter()
    {

    }
    public override void Update()
    {
        owner.FollowTarget();
        // if (owner.followTarget != null && Vector3.Distance(owner.transform.position, owner.followTarget.position) > stopDistance)
        // {
        //     owner.FollowTarget();
        // }
        // else{
        //     owner.characterController.SimpleMove(Vector3.zero);
        // }
        // owner.UpdateAnimationParams();
        
        if(owner.followTarget == null){
            stateMachine.ChangeState(nextState);
        }
    }

    public override void Exit()
    {
        
    }
}
