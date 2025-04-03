using UnityEngine;
using UnityEngine.AI;

public interface IMoveStrategy
{
    void Move();
}

public class MovePathfinding : IMoveStrategy
{
    private Unit owner;

    private float calcCD = 1f;
    private float _calcCD = 1f;

    public MovePathfinding(Unit owner, float pathCalculationDelay = 1f)
    {
        //this.agent = agent;
        calcCD = pathCalculationDelay;
        this.owner = owner;

        _calcCD = pathCalculationDelay;
        //path = new NavMeshPath();
    }

    public void Move()
    {
        if (_calcCD <= 0)
        {
            owner.CalculatePathToTarget();
            _calcCD = calcCD;
        }
        else
        {
            _calcCD -= Time.deltaTime;
        }
    }
}


public class PhysicsFollow : IMoveStrategy
{
    private float speed = 500;
    private Rigidbody2D rb;
    public PhysicsFollow(float speed, Rigidbody2D rb)
    {
        this.rb = rb;
        this.speed = speed;

        if (rb.gameObject.GetComponent<NavMeshAgent>() != null)
        {
            GameObject.Destroy(rb.gameObject.GetComponent<NavMeshAgent>());
        }
    }
    
    public void Move()
    {
        rb.AddForce(rb.transform.forward * speed * Time.fixedDeltaTime);
    }
}