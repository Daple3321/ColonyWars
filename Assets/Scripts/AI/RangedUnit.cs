using UnityEngine;

public class RangedUnit : Unit
{
    public UnitState idleState;
    public UnitState retreatState;
    public UnitState attackState;
    public UnitState followState;

    void Start()
    {
        base.Init();
        
        attackState = new AttackState
        {
            stateMachine = stateMachine,
            owner = this,
        };
        retreatState = new HomeRetreatState
        {
            stateMachine = stateMachine,
            owner = this,
        };
        idleState = new IdleState
        {
            stateMachine = stateMachine,
            owner = this,
        };
        followState = new FollowState
        {
            stateMachine = stateMachine,
            owner = this,
        };
        
        attackState.Init(idleState, retreatState);
        retreatState.Init(idleState, attackState);
        idleState.Init(attackState, retreatState);
        followState.Init(idleState, retreatState);

        stateMachine.ChangeState(idleState);
    }

    public override void StartFollowing()
    {
        stateMachine.ChangeState(followState);
    }
    public override void StopFollowing()
    {
        stateMachine.ChangeState(idleState);
        followTarget = null;
    }
    
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     homePos = GameController.GetPointOnTerrain(homePos);
        // }
    }
    
    public override void Death()
    {
        onUnitDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
