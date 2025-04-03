using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StateMachine))]
public class Unit : MonoBehaviour, IDamageable
{
    public float health;
    public float maxHealth;

    public float speed;
    public float maxSpeed;

    public float damage;


    public Affiliation affiliation;
    public Transform currentTarget;
    public StateMachine stateMachine;
    private NavMeshAgent agent;

    void Awake()
    {
        //enabled = false;
    }

    public virtual void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        //currentTarget = GameController.p.transform;

        stateMachine.Init(new IdleState
        {
            stateMachine = stateMachine,
            owner = this,
        });

        enabled = true;
        Debug.Log("Enabled: " + enabled);

        agent.SetDestination(currentTarget.position);
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
