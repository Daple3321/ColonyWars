using System;
using System.Collections;
using PrimeTween;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public abstract class Unit : MonoBehaviour, IDamageable
{
    public string unitName;
    
    public float health;
    public float maxHealth;
    
    [Space(5), Header("Movement")]
    public float speed;
    public float maxSpeed;
    public float rotationSpeed;
    public float fallSpeed;
    public IMoveStrategy moveStrategy;
    
    [Space(10), Header("Attack Settings")]
    [SerializeReference] public Attack currentAttack;
    [SerializeReference] public AttackData attackData;
    [Space(10)]
    public float damage;
    public float attackDistance = 1f;
    public float attackDuration;
    public float attackSpeed;
    protected float _attackSpeed;
    public float concentration;
    public bool isAttacking;


    [Space(10), Header("Home Point")]
    public Vector3 homePos;
    public float homeRadius;
    public float targetStopDistance = 2f;

    public float enemyCheckDelay = 1f;
    public LayerMask enemiesMask;
    public LayerMask attackHitMask;

    public Squad squad;
    protected CharacterController characterController;
    private WorldBar healthBar;
    private Animator animator;
    public Transform attackPoint;
    public Transform attackTarget = null;
    public Transform followTarget = null;
    public Affiliation affiliation;
    public StateMachine stateMachine;

    void Awake(){
        GameController.OnGameStarted += Init;
        enabled = false;
    }
    void Start(){
        
    }
    void OnDestroy(){ 
        GameController.OnGameStarted -= Init;
    }

    public virtual void Init()
    {
        //agent = GetComponent<NavMeshAgent>();
        //path = new NavMeshPath();
        //agent.speed = speed;
        //currentTarget = GameController.p.transform;
        //agent.SetDestination(currentTarget.position);
        gameObject.name = unitName;
        squad = null;
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        homePos = transform.position;

        stateMachine.Init();
        
        InitUI();
        
        enabled = true;
    }
    
    protected virtual void InitUI()
    {
        healthBar = Instantiate(GameAssets.worldBar, GameController.i.worldCanvas.transform).GetComponent<WorldBar>();
        healthBar.Init(affiliation);
        healthBar.InitWorldBar(transform, GameController.i.worldCanvas);
        healthBar.offset.y = transform.localScale.y + 1.4f;
        healthBar.UpdateBar(health, maxHealth);
    }
    
    protected Coroutine attackRoutine;
    public virtual void HandleAttacking() 
    {
        if(CanAttack())
        {
            attackRoutine = StartCoroutine(Attack());
        }
    }
    public virtual IEnumerator Attack()
    {
        isAttacking = true;
        currentAttack.OneShotAttack();
        // Attack effects&animations here

        float attackDur = this.attackDuration;
        while(attackDur > 0)
        {
            currentAttack.ConstantAttack();
            attackDur -= Time.deltaTime;
            yield return null;
        }
        
        _attackSpeed = this.attackSpeed;
        while(_attackSpeed > 0)
        {
            _attackSpeed -= Time.deltaTime;
            yield return null;
        }
        
        isAttacking = false;
    }
    public virtual bool CanAttack(){ return isAttacking ? false : true && _attackSpeed <= 0; }
    public virtual bool IsAttacking() { return false;}
    
    public virtual void RegisterToSquad(Squad squad)
    {
        this.squad = squad;
        onRegiesterToSquad?.Invoke(squad);
    }
    public virtual void UnregisterFromSquad()
    {
        this.squad = null;
        followTarget = null;
        onUnregisterFromSquad?.Invoke();
    }
    public bool InSquad(){
        return squad == null ? false : true;
    }

    public abstract void ConfigureStates();
    public abstract void StartFollowing();
    public abstract void StopFollowing();

    public virtual void FollowTarget()
    {
        if (followTarget != null && Vector3.Distance(transform.position, followTarget.position) > targetStopDistance)
        {
            MoveTo(followTarget.position);
            RotateTo(followTarget.position);
        }
        else{
            ResetVelocity();
        }
        UpdateAnimationParams();
        
        //MoveTo(followTarget.position);
    }
    public virtual void MoveToAttackTarget()
    {
        if(HasTarget() && DistanceToTarget() > attackDistance)
        {
            MoveTo(attackTarget.position);
            RotateTo(attackTarget.position);
        }
        else{
            ResetVelocity();
        }
        UpdateAnimationParams();
        
        //MoveTo(attackTarget.position);
    }
    public void ResetVelocity(){
        characterController.SimpleMove(Vector3.zero);
    }
    
    public virtual void MoveToHome()
    {
        MoveTo(homePos);
        RotateTo(homePos);
    }
    
    Vector3 moveDir = new Vector3(0, 0, 0);
    protected virtual void MoveTo(Vector3 target)
    {
        moveDir = (target - transform.position).normalized;
        if (!characterController.isGrounded)
        {
            moveDir.y = -fallSpeed;
        }
        characterController.Move(moveDir * speed * Time.deltaTime);
        
        //Debug.DrawRay(transform.position, moveDir*10, Color.cyan);
        //float speedX = Mathf.Clamp(characterController.velocity.x, -1, 1);
        //float speedZ = Mathf.Clamp(characterController.velocity.z, -1, 1);
        //animator.SetFloat("Speed X", dir.x, 0.1f, Time.deltaTime);
        //animator.SetFloat("Speed Z", dir.z, 0.1f, Time.deltaTime);
    }
    public virtual void RotateTo(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        Vector3 lookDir = Quaternion.AngleAxis(-90, Vector3.up) * Vector3.Cross(Vector3.up, dir);
        Quaternion lookRot = Quaternion.LookRotation(lookDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        
        Debug.DrawRay(transform.position, dir*8, Color.red);
        //Debug.DrawRay(transform.position, lookDir*10, Color.cyan);    
    }
    public virtual void UpdateAnimationParams()
    {
        //Debug.DrawRay(transform.position, moveDir*10, Color.cyan);
        // animator.SetFloat("Speed X", moveDir.x, 0.1f, Time.deltaTime);
        // animator.SetFloat("Speed Z", moveDir.z, 0.1f, Time.deltaTime);
        
        float speedX = Mathf.Clamp(characterController.velocity.x, -1, 1);
        float speedZ = Mathf.Clamp(characterController.velocity.z, -1, 1);
        animator.SetFloat("Speed X", speedX, 0.1f, Time.deltaTime);
        animator.SetFloat("Speed Z", speedZ, 0.1f, Time.deltaTime);
    }
    
    public void SetHome(Vector3 newHomePos)
    {
        homePos = newHomePos;
    }
    public float DistanceToHome()
    {
        return Vector3.Distance(homePos, transform.position);
    }
    public float DistanceToTarget() // когда target уничтожается всё ломается.
    {
        //Debug.Log($"Dist to target: {Vector3.Distance(transform.position, currentTarget.position)}");
        return Vector3.Distance(transform.position, attackTarget.position);
    }
    public bool HasTarget()
    {
        return attackTarget != null;
    }
    
    public bool CheckForEnemies()
    {
        Collider[] hitColliders = {};
        if(homeRadius > attackDistance){
           hitColliders = Physics.OverlapSphere(homePos, homeRadius, enemiesMask);
        }
        else if(homeRadius < attackDistance){
            hitColliders = Physics.OverlapSphere(transform.position, attackDistance, enemiesMask);
        }
        
        if (hitColliders.Length > 0){
            attackTarget = FindClosestObject(hitColliders).transform;
            //Debug.Log($"Closest enemy: {currentTarget.name}");
            return true;
        }
        else{
            attackTarget = null;
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
        healthBar.UpdateBar(health, maxHealth);
        onUnitHealthChanged?.Invoke(health, maxHealth);
        if (health <= 0)
        {
            Death();
        }
    }

    public virtual void Death() { Destroy(healthBar.gameObject); }

    public Action<Unit> onUnitDeath;
    public Action<Squad> onRegiesterToSquad;
    public Action onUnregisterFromSquad;
    public Action<float, float> onUnitHealthChanged;
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Vector3 labelPos1 = new Vector3(homePos.x + 0.5f, homePos.y, homePos.z + homeRadius + 0.1f);
        Handles.Label(labelPos1, "Home Radius");
        Handles.DrawWireDisc(new Vector3(homePos.x, homePos.y, homePos.z), Vector3.up, homeRadius);
        
        Handles.color = Color.red;
        Vector3 labelPos2 = new Vector3(transform.position.x + 0.5f, transform.position.y+1, transform.position.z + attackDistance + 0.1f);
        Handles.Label(labelPos2, "Attack distance");
        Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.up, attackDistance);


        Vector3 labelPos3 = new Vector3(transform.position.x + 1.5f, transform.position.y, transform.position.z);
        Handles.Label(labelPos3, $"Current state: {stateMachine.currentState}\n");
    }
#endif
}
