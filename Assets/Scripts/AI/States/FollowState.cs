using UnityEngine;

public class FollowState : UnitState
{
    public float stopDistance = 2f;
    
    public override void Enter()
    {

    }
    public override void Update()
    {
        if (Vector3.Distance(owner.transform.position, owner.followTarget.position) > stopDistance)
        {
            owner.FollowTarget();
        }
    }

    public override void Exit()
    {
        
    }
}
