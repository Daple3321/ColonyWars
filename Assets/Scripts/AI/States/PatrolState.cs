using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PatrolState : UnitState
{
    private float enemyCheckDelay;
    
    public List<Vector3> patrolRoute;
    public int currentPoint = 0;
    public float waitTime = 7f;
    private float _waitTime = 7f;
    
    public EnemyColony parentColony;
    public override void Enter()
    {
        this.enemyCheckDelay = owner.enemyCheckDelay;
        _waitTime = waitTime;
        
        patrolRoute = new List<Vector3>();
        if(parentColony != null)
        {
            foreach(var building in parentColony.buildings)
            {
                Vector2 bPos = new Vector2(building.transform.position.x, building.transform.position.z);
                patrolRoute.Add(GameController.RandomPointInCircleTerrain(bPos, 2.5f, 10f));
            }
        }
        else{
            patrolRoute.Add(GameController.RandomPointInCircleTerrain(owner.transform.position, 2.5f, 10f));
        }
    }
    public override void Update()
    {
        HandlePatroling();
        
        owner.movement.UpdateAnimationParams();
        
        if (enemyCheckDelay > 0){
            enemyCheckDelay -= Time.deltaTime;
        }
        else
        {
            enemyCheckDelay = owner.enemyCheckDelay;
            if (owner.CheckForEnemies()) // нашли врага в радиусе
            {
                stateMachine.ChangeState(nextState);
            }
        }

        if (owner.DistanceToHome() > 1.5f) // если далеко от дома
        {
            base.Back();
        }
    }
    
    private void HandlePatroling()
    {
        if(_waitTime > 0){
            _waitTime -= Time.deltaTime;
        }
        else{
            _waitTime = waitTime;
            
            NextPoint();
        }
        
        if (owner.DistanceToHome() <= 1.5f)
        {
            owner.movement.ResetVelocity();
        }
        else{
            owner.MoveToHome();
        }
    }
    private void NextPoint()
    {
        if(currentPoint < patrolRoute.Count-1){
            currentPoint++;
        }
        else{
            currentPoint = 0;
        }
        
        owner.SetHome(patrolRoute[currentPoint]);
    }

    public override void Exit()
    {
        
    }
}
