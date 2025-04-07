using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public abstract class Unit : MonoBehaviour, IDamageable
{
    public float health;
    public float maxHealth;

    public float speed;
    public float maxSpeed;
    public float rotationSpeed;
    public float fallSpeed;
    public IMoveStrategy moveStrategy;

    public float damage;
    public float attackDistance = 1f;


    public float concentration;
    [Space(10), Header("Home Point")]
    public Vector3 homePos;
    public float homeRadius;

    public float enemyCheckDelay = 1f;
    public LayerMask enemiesMask;


    private CharacterController characterController;
    public Transform currentTarget = null;
    public Transform followTarget = null;
    public Affiliation affiliation;
    public StateMachine stateMachine;

    void Awake()
    {
        //enabled = false;
    }

    public virtual void Init()
    {
        //agent = GetComponent<NavMeshAgent>();
        //path = new NavMeshPath();
        //agent.speed = speed;
        //currentTarget = GameController.p.transform;
        //agent.SetDestination(currentTarget.position);
        characterController = GetComponent<CharacterController>();
        homePos = transform.position;
        //homePos = GameController.GetPointOnTerrain(homePos);

        stateMachine.Init();

        enabled = true;
    }

    public virtual void HandleAttacking()
    {
        
    }
    
    public virtual void RegisterToSquad(Squad squad)
    {

    }
    public virtual void UnregisterFromSquad()
    {
        
    }

    public abstract void ConfigureStates();
    public abstract void StartFollowing();
    public abstract void StopFollowing();

    public virtual void FollowTarget()
    {
        MoveTo(followTarget.position);
        RotateTo(followTarget.position);
    }
    
    public virtual void MoveToCurrentTarget()
    {
        MoveTo(currentTarget.position);
        RotateTo(currentTarget.position);
    }
    
    public virtual void MoveToHome()
    {
        MoveTo(homePos);
        RotateTo(homePos);
    }
    
    protected virtual void MoveTo(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        if (!characterController.isGrounded)
        {
            dir.y = -fallSpeed;
        }
        characterController.Move(dir * speed * Time.deltaTime);
    }
    protected virtual void RotateTo(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        Vector3 lookDir = Quaternion.AngleAxis(-90, Vector3.up) * Vector3.Cross(Vector3.up, dir);
        Quaternion lookRot = Quaternion.LookRotation(lookDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        
        Debug.DrawRay(transform.position, dir*8, Color.red);
        Debug.DrawRay(transform.position, lookDir*10, Color.cyan);    
    }

    public void SetHome(Vector3 newHomePos)
    {
        homePos = newHomePos;
    }
    public float DistanceToHome()
    {
        return Vector3.Distance(homePos, transform.position);
    }
    public float DistanceToTarget()
    {
        //Debug.Log($"Dist to target: {Vector3.Distance(transform.position, currentTarget.position)}");
        return Vector3.Distance(transform.position, currentTarget.position);
    }

    public bool CheckForEnemies()
    {
        var hitColliders = Physics.OverlapSphere(transform.position, homeRadius, enemiesMask);
        if (hitColliders.Length > 0)
        {
            currentTarget = FindClosestObject(hitColliders).transform;
            //Debug.Log($"Closest enemy: {currentTarget.name}");
            return true;
        }
        else
        {
            currentTarget = null;
            //Debug.Log("Enemies not found in radius.");
            return false;
        }
    }

    public GameObject FindClosestObject(Collider[] colliders)
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (Collider go in colliders)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go.gameObject;
                distance = curDistance;
                // if (distance < TargetRange /*&& !go.transform.GetComponent<FriendlyUnit_AI>().isDead*/)
                //     OptimalTarget = go.transform;
            }
        }
        return closest;
    }
    
    public virtual void ChangeSpeed(float speed)
    {
        this.speed = speed;
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

    public virtual void Death() { }


    public Action<Unit> onUnitDeath;
    
    
    
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Vector3 labelPos1 = new Vector3(homePos.x - 0.5f, homePos.y, homePos.z + homeRadius + 0.1f);
        Handles.Label(labelPos1, "Home Radius");
        Handles.DrawWireDisc(new Vector3(homePos.x, homePos.y, homePos.z), Vector3.up, homeRadius);


        Vector3 labelPos3 = new Vector3(transform.position.x + 1.5f, transform.position.y, transform.position.z);
        Handles.Label(labelPos3, $"Current state: {stateMachine.currentState}\n");
    }
#endif
}
