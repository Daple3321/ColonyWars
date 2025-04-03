using UnityEngine;

public class FollowState : UnitState
{    
    public override void Enter()
    {
        //pathCalculationDelay += Random.Range(-0.4f, 0.4f);
        //_pathDelay = pathCalculationDelay;
    }
    public override void Update()
    {
        owner.moveStrategy.Move();
    }

    public override void Exit()
    {
        
    }
}
