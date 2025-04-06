using UnityEngine;

public class Enemy : Unit
{
    public UnitState followState;
    public UnitState attackState;

    void Start()
    {
        base.Init();
        moveStrategy = new MovePathfinding(this, 0.6f);

        attackState = new AttackState
        {
            stateMachine = stateMachine,
            nextState = attackState,
            owner = this,
        };

        followState = new FollowState
        {
            stateMachine = stateMachine,
            nextState = attackState,
            owner = this,
        };

        Invoke(nameof(StartStates), Random.Range(0.1f, 3f));

    }

    void StartStates() // delete later
    {
        //currentTarget = GameController.p.transform;
        stateMachine.ChangeState(followState);
    }

    public override void Death()
    {
        onUnitDeath?.Invoke(this);
        Destroy(gameObject);
    }

    public override void StartFollowing()
    {
        
    }

    public override void StopFollowing()
    {
        
    }
}
