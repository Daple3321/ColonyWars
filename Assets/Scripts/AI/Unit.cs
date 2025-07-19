using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEditor;
using UnityEngine;
using static EntityStatType;

[RequireComponent(typeof(StateMachine))]
public abstract class Unit : MonoBehaviour, IDamageable, IClickable
{
    public string unitName;
    
    public float health;
    
    [Space(5), Header("Movement")]
    public UnitMovement movement;
    
    [SerializedDictionary("Stat Type", "Unit Stat")]
    public SerializedDictionary<EntityStatType, Stat> stats;
    
    public UnitCombat combat;
    public StunHandler stun;
    
    [Space(10), Header("Leveling")]
    public LevelSystem levelSystem;


    [Space(10), Header("Home Point")]
    public Vector3 homePos;
    public float homeRadius;
    public float targetStopDistance = 2f;

    public float enemyCheckDelay = 1f;
    public LayerMask enemiesMask;
    public LayerMask attackHitMask;
    
    public UnitData data;
    
    private Squad _squad;
    public Squad squad {
    get{
        return _squad; 
        }
    set{
        _squad = value;
        //Debug.Log("Squad has been assigned");
    }
    }
    protected CharacterController characterController;
    public GameObject characterObject;
    protected WorldBar healthBar;
    public Animator animator;
    public AnimationEvents animationEvents;
    protected Material mat;
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
        gameObject.name = data.unitName;
        squad = null;
        characterController = GetComponent<CharacterController>();
        characterObject = transform.Find("CHARACTER").gameObject;
        animator = GetComponent<Animator>();
        animationEvents = GetComponent<AnimationEvents>();
        animationEvents.Init();
        mat = GetComponentInChildren<Renderer>().material;
        homePos = transform.position;
        
        levelSystem = new LevelSystem();
        stun = new StunHandler();
        stun.OnStun += OnStun;
        stun.OnStunEnd += OnStunEnd;
        
        movement.Init(this);

        InitPatrolRoute();
        stateMachine.Init(this);
        
        InitUI();
        
        enabled = true;
    }
    
    public void ChangeLevel(int level)
    {
        levelSystem.ChangeLevel(level);
        healthBar.SetDescription($"{data.unitName} Lv.{levelSystem.GetLevel()}");
    }
    
    protected virtual void InitUI()
    {
        //healthBar = Instantiate(GameAssets.worldBar, GameController.i.worldCanvas.transform).GetComponent<WorldBar>();
        healthBar = WorldUI.i.CreateWorldBar();
        healthBar.Init(affiliation);
        healthBar.InitWorldBar(transform, GameController.i.worldCanvas, data.unitName);
        healthBar.SetDescription($"{data.unitName} Lv.{levelSystem.GetLevel()}");
        healthBar.offset.y = transform.localScale.y+transform.localScale.y+0.15f;
        healthBar.parentTransform.position = transform.position + healthBar.offset;
        healthBar.parentTransform.SetParent(transform);
        healthBar.UpdateBar(health, stats[maxHealth].Value);
    }
    
    protected virtual void UpdateUI()
    {
        if(healthBar.CheckDisctance() && !healthBar.gameObject.activeInHierarchy){
            healthBar.gameObject.SetActive(true);
        }
        else if(!healthBar.CheckDisctance() && healthBar.gameObject.activeInHierarchy){
            healthBar.gameObject.SetActive(false);
        }
    }
    private float _uiUpdateDelay = 1f;
    protected void HandleUI(){
        if(_uiUpdateDelay > 0){
            _uiUpdateDelay -= Time.deltaTime;
        }
        else{
            UpdateUI();
            _uiUpdateDelay = 1f;
        }
    }
    
    protected virtual void Update(){
        HandleUI();
        stun.HandleStun();
    }
    
    protected virtual void OnStun()
    {
        if(combat.isAttacking){
            combat.CancelAttackInstant();
        }
        
        movement.ResetVelocity();
        
        GameObject go = Instantiate(GameAssets.stunEffect, transform.position+new Vector3(0, 1.7f, 0), Quaternion.identity);
        go.transform.forward = transform.up;
        go.transform.SetParent(transform);
        Destroy(go, stun.GetStunTime());
    }
    protected virtual void OnStunEnd(){
        
    }
    
    public void OnClick(Player caller)
    {
        if(affiliation == Affiliation.Player){
            GameController.p.squadManager.SelectUnit(this);
        }
    }
    public void RegisterToSquad(Squad squad)
    {
        this.squad = squad;
        onRegiesterToSquad?.Invoke(squad);
        //Debug.Log($"REGISTERING TO SQUAD {squad.squadOwner}");
    }
    public void UnregisterFromSquad()
    {
        //Debug.Log($"UNREGESTERING FROM SQUAD {squad.squadOwner}");
        
        this.squad = null;
        followTarget = null;
        onUnregisterFromSquad?.Invoke();
    }
    public bool InSquad(){
        if(squad != null){
            return true;
        }
        else{
            return false;
        }
        //return squad != null;
    }
    public bool InSquad(Squad squadCompare){
        if(squad != null && squad == squadCompare){
            return true;
        }
        else{
            return false;
        }
        //return squad != null;
    }

    public abstract void ConfigureStates();
    public abstract void StartFollowing();
    public abstract void StopFollowing();
    
    public List<Vector3> patrolRoute;
    public int currentPatrolPoint = 0;
    public int maxPatrolPoints = 15;
    public virtual void InitPatrolRoute(Colony parentColony = null)
    {
        if(parentColony != null)
        {
            if(parentColony.buildings.Count >= maxPatrolPoints)
            {
                patrolRoute = new List<Vector3>(maxPatrolPoints);
                for(int i = 0; i < maxPatrolPoints; i++)
                {
                    Vector2 bPos = new Vector2(parentColony.buildings[i].transform.position.x, parentColony.buildings[i].transform.position.z);
                    patrolRoute.Add(GameController.RandomPointInCircleTerrain(bPos, 2.5f, 10f));
                }
            }
            else if(parentColony.buildings.Count < maxPatrolPoints)
            {
                patrolRoute = new List<Vector3>(parentColony.buildings.Count);
                for(int i = 0; i < parentColony.buildings.Count; i++)
                {
                    Vector2 bPos = new Vector2(parentColony.buildings[i].transform.position.x, parentColony.buildings[i].transform.position.z);
                    patrolRoute.Add(GameController.RandomPointInCircleTerrain(bPos, 2.5f, 10f));
                }
            }
        }
        else{
            patrolRoute = new List<Vector3>
            {
                GameController.RandomPointInCircleTerrain(transform.position, 2.5f, 10f)
            };
        }
        
        ArrayExtensionMethods.ShuffleList(patrolRoute);
    }
    
    public virtual void FollowTarget()
    {
        if (followTarget != null && Vector3.Distance(transform.position, followTarget.position) > targetStopDistance)
        {
            movement.MoveTo(followTarget.position);
            movement.RotateTo(followTarget.position);
        }
        else{
            movement.ResetVelocity();
        }
        movement.UpdateAnimationParams();
        
        //MoveTo(followTarget.position);
    }
    public virtual void MoveToAttackTarget()
    {
        if(HasTarget() && DistanceToTarget() > combat.stats[attackDistance].Value)
        {
            movement.MoveTo(attackTarget.position);
            movement.RotateTo(attackTarget.position);
        }
        else{
            movement.ResetVelocity();
        }
        movement.UpdateAnimationParams();
    }
    
    public virtual void MoveToHome()
    {
        movement.MoveTo(homePos);
        movement.RotateTo(homePos);
    }
    public virtual void OnArrivedToHome()
    {
        OnArrivedHome?.Invoke(this);
    }
    public void SetHome(Vector3 newHomePos)
    {
        homePos = newHomePos;
    }
    public float DistanceToHome()
    {
        //eturn Vector3.SqrMagnitude(homePos - transform.position);
        return Vector3.Distance(homePos, transform.position);
    }
    public float DistanceToTarget() // когда target уничтожается всё ломается.
    {
        //Debug.Log($"Dist to target: {Vector3.Distance(transform.position, currentTarget.position)}");
        return Vector3.Distance(transform.position, attackTarget.position);
    }
    public bool HasTarget()
    {
        return attackTarget != null && attackTarget.gameObject.activeInHierarchy;
    }
    
    public bool CheckForEnemies()
    {
        Collider[] hitColliders = {};
        if(homeRadius > combat.stats[attackDistance].Value){
           hitColliders = Physics.OverlapSphere(homePos, homeRadius, enemiesMask);
        }
        else if(homeRadius < combat.stats[attackDistance].Value){
            hitColliders = Physics.OverlapSphere(transform.position, combat.stats[attackDistance].Value, enemiesMask);
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
    
    public void AddHealth(float amount)
    {
        health += amount;
        health = Math.Clamp(health, 0, stats[maxHealth].Value);
        HealthChanged();
        
        Flash(Color.green, 0.3f);
    }
    public virtual UniTask<DamageResult> TakeDamage<T>(float damage, T source, Vector3 knockback = new Vector3(), DamageType damageType = DamageType.Melee)
    {
        float finalDamage = CalculateDamageWithResists(damage, damageType);
        health -= finalDamage;
        //health -= damage;
        
        movement.AddForce(knockback);
        
        Flash(Color.red, 0.2f);
            
        if(affiliation == Affiliation.Player){
            WorldUI.i.DamagePopup(transform.position+new Vector3(0, 1.65f, 0), Color.red)
            .text = damage.ToString("F1");
        }
        else{
            WorldUI.i.DamagePopup(transform.position+new Vector3(0, 1.65f, 0), Color.white)
            .text = damage.ToString("F1");
        }
        
        HealthChanged();
        return UniTask.FromResult(DamageResult.Dealt);
    }
    protected float CalculateDamageWithResists(float baseDamage, DamageType damageType)
    {
        float finalDamage = baseDamage;
        switch(damageType) {
            case DamageType.Explosive:
                finalDamage -= baseDamage * (stats[explosiveDamageResist].Value / 100);
                return finalDamage;
            case DamageType.Melee:
                finalDamage -= baseDamage * (stats[meleeDamageResist].Value / 100);
                return finalDamage;
            case DamageType.Ranged:
                finalDamage -= baseDamage * (stats[rangedDamageResist].Value / 100);
                return finalDamage;
            case DamageType.Destructive:
                finalDamage -= baseDamage * (stats[destructiveDamageResist].Value / 100);
                return finalDamage;
            
            default:
                return finalDamage;
        }
    }
    public (float health, float maxHealth) GetHealth(){
        return (health, stats[maxHealth].Value);
    }
    protected virtual void HealthChanged()
    {
        healthBar.UpdateBar(health, stats[maxHealth].Value);
        onUnitHealthChanged?.Invoke(health, stats[maxHealth].Value);
        if (health <= 0)
        {
            Death();
        }
    }
    
    Sequence colorSeq;
    public void Flash(Color flashColor, float duration = 0.2f)
    {
        colorSeq.Complete();
        colorSeq = Sequence.Create()
            .Chain(Tween.MaterialColor(mat, flashColor, duration))
            .Chain(Tween.MaterialColor(mat, Color.white, duration));
    }

    public virtual void Death() { 
        if(!data.lootTable.IsEmpty()){
            data.lootTable.DropLoot(transform.position + new Vector3(0, 1.5f, 0));
        }
        
        onUnitDeath?.Invoke(this);
        
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }

    public event Action<Unit> onUnitDeath;
    public Action<Squad> onRegiesterToSquad;
    public Action onUnregisterFromSquad;
    public Action<float, float> onUnitHealthChanged;
    public event Action<Unit> OnArrivedHome;
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.green;
        Vector3 labelPos1 = new Vector3(transform.position.x+1.5f, transform.position.y+3f, transform.position.z);
        //Handles.Label(labelPos1, "Home Radius");
        Handles.Label(labelPos1, $"Distance to HOME: {DistanceToHome():F2}");
        Handles.DrawWireDisc(new Vector3(homePos.x, homePos.y, homePos.z), Vector3.up, homeRadius);
        
        Handles.DrawWireCube(new Vector3(homePos.x, homePos.y, homePos.z), new Vector3(0.2f, 0.2f, 0.2f));

        Vector3 labelPos3 = new Vector3(transform.position.x + 1.5f, transform.position.y, transform.position.z);
        Handles.Label(labelPos3, $"State: {stateMachine.currentState}\n");
    }

#endif
}

public class StunHandler
{
    private float stunTime;    
    private float maxStunTime = 4f;
    
    public event Action OnStun;
    public event Action OnStunEnd;
    private bool stunEndEventSent = false;
    public StunHandler(float maxStunTime = 4f)
    {
        stunTime = 0f;
        this.maxStunTime = maxStunTime;
    }
    
    public void AddStun(float seconds){
        stunTime += seconds;
        stunTime = Mathf.Clamp(stunTime, 0, maxStunTime);
        
        OnStun?.Invoke();
        stunEndEventSent = false;
    }
    
    public void HandleStun()
    {
        if(stunTime > 0){
            stunTime -= Time.deltaTime;
        }
        else if(stunTime < 0 && !stunEndEventSent){
            OnStunEnd?.Invoke();
            stunEndEventSent = true;
        }
    }
    
    public bool IsStunned(){
        return stunTime > 0;
    }
    
    public float GetStunTime(){
        return stunTime;
    }
}