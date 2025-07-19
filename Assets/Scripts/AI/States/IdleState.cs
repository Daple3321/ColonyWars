using UnityEngine;

[CreateAssetMenu(fileName = "New Idle State", menuName = "AI/States/Idle state")]
public class IdleState : UnitState
{
    private float enemyCheckDelay;
    
    public UnitState OnEnemyFound;
    public UnitState OnFarFromHome;

    public override void Enter(StateMachine stateMachine)
    {
        this.enemyCheckDelay = stateMachine.owner.enemyCheckDelay;
    }
    public override void UpdateState(StateMachine stateMachine)
    {
        stateMachine.owner.movement.UpdateAnimationParams();
        
        if (enemyCheckDelay > 0)
        {
            enemyCheckDelay -= Time.deltaTime;
        }
        else
        {
            enemyCheckDelay = stateMachine.owner.enemyCheckDelay;
            if (stateMachine.owner.CheckForEnemies()) // нашли врага в радиусе
            {
                stateMachine.ChangeState(OnEnemyFound);
            }
        }

        if (stateMachine.owner.DistanceToHome() > stateMachine.owner.targetStopDistance) // если далеко от дома
        {
            stateMachine.ChangeState(OnFarFromHome);
        }
    }

    public override void Exit()
    {
        
    }

}
