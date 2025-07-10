using UnityEngine;

[CreateAssetMenu(fileName = "New Follow State", menuName = "AI/States/Follow state")]
public class FollowState : UnitState
{
    public float stopDistance = 2f;
    
    public UnitState OnNoFollowTarget;
    
    public override void Enter(StateMachine stateMachine)
    {

    }
    public override void Update(StateMachine stateMachine)
    {
        stateMachine.owner.FollowTarget();
        // if (owner.followTarget != null && Vector3.Distance(owner.transform.position, owner.followTarget.position) > stopDistance)
        // {
        //     owner.FollowTarget();
        // }
        // else{
        //     owner.characterController.SimpleMove(Vector3.zero);
        // }
        // owner.UpdateAnimationParams();
        
        if(stateMachine.owner.followTarget == null){
            stateMachine.ChangeState(OnNoFollowTarget);
        }
    }

    public override void Exit()
    {
        
    }
}
