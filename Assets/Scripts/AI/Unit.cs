using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StateMachine))]
public abstract class Unit : MonoBehaviour, IDamageable
{
    public float health;
    public float maxHealth;

    public float speed;
    public float maxSpeed;
    public IMoveStrategy moveStrategy;

    public float damage;


    public float concentration;

    public Affiliation affiliation;
    public Transform currentTarget;
    public StateMachine stateMachine;
    public NavMeshAgent agent;
    public NavMeshPath path;

    void Awake()
    {
        //enabled = false;
    }

    public virtual void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        agent.speed = speed;
        //currentTarget = GameController.p.transform;

        stateMachine.Init(new IdleState
        {
            stateMachine = stateMachine,
            owner = this,
        });

        enabled = true;
        Debug.Log("Enabled: " + enabled);

        //agent.SetDestination(currentTarget.position);
    }

    public virtual void CalculatePathToTarget(Vector3 target = new Vector3())
    {
        if (target == Vector3.zero)
        {
            if (agent.CalculatePath(currentTarget.position, path) == true)
            {
                agent.SetPath(path);
            }
            else
            {
                Debug.Log("Path not found.");
            }
        }
        else
        {
            if (agent.CalculatePath(target, path) == true)
            {
                agent.SetPath(path);
            }
            else
            {
                Debug.Log("Path not found.");
            }
        }
    }

    public virtual void ChangeSpeed(float speed)
    {
        this.speed = speed;
        agent.speed = speed;
    }

    public virtual void TakeDamage(float damage, float knockback = 0f)
    {
        health -= damage;
        HealthChanged();
    }
    protected virtual void HealthChanged()
    {
        if (health <= 0)
        {
            Death();
        }
    }

    public virtual void Death(){}
}
