using UnityEngine;

[CreateAssetMenu(fileName = "New Patrol State", menuName = "AI/States/Patrol state")]
public class PatrolState : UnitState
{
    private float enemyCheckDelay;
    
    public float waitTime = 7f;
    private float _waitTime = 7f;
    
    public UnitState OnEnemyFound;
    public UnitState OnFarFromHome;
    
    public void InitRoute(StateMachine stateMachine)
    {
        if(stateMachine.owner is Enemy enemy){
            stateMachine.owner.InitPatrolRoute(enemy.parentColony);
        }
    }
    
    public override void Enter(StateMachine stateMachine)
    {
        this.enemyCheckDelay = stateMachine.owner.enemyCheckDelay;
        _waitTime = waitTime;
    }
    public override void UpdateState(StateMachine stateMachine)
    {
        HandlePatroling(stateMachine.owner);
        
        stateMachine.owner.movement.UpdateAnimationParams();
        
        if (enemyCheckDelay > 0){
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

        if (stateMachine.owner.DistanceToHome() > 1.5f) // если далеко от дома
        {
            stateMachine.ChangeState(OnFarFromHome);
        }
    }
    
    private void HandlePatroling(Unit owner)
    {
        if(_waitTime > 0){
            _waitTime -= Time.deltaTime;
        }
        else{
            _waitTime = waitTime;
            
            NextPoint(owner);
        }
        
        if (owner.DistanceToHome() <= 1.5f)
        {
            owner.movement.ResetVelocity();
        }
        else{
            owner.MoveToHome();
        }
    }
    private void NextPoint(Unit owner)
    {
        if(owner.currentPatrolPoint < owner.patrolRoute.Count-1){
            owner.currentPatrolPoint++;
        }
        else{
            owner.currentPatrolPoint = 0;
        }
        
        owner.SetHome(owner.patrolRoute[owner.currentPatrolPoint]);
    }

    public override void Exit()
    {
        
    }
}
