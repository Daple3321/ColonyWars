using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public abstract class Defense : MonoBehaviour
{
    public Unit currentTarget;
    public Player playerTarget = null;
    [SerializeField] private List<Unit> targetsInRange = new();
    [SerializeField] private EnemyDetection detection;
    public Transform attackPoint;
    
    [Space(10)]
    [SerializedDictionary("Stat Type", "Tower Stat")]
    public SerializedDictionary<EntityStatType, Stat> stats;
    
    public Affiliation affiliation;
    public Affiliation whoIsEnemy;
    public LayerMask attackHitMask;
    public enum TargetingStyle{
        First,
        Last,
        Strong,
        Weak,
    }
    public TargetingStyle targetingStyle = TargetingStyle.First;

    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        detection.Init(this, whoIsEnemy);
    }
    
    public virtual bool CanAttack(){ return currentTarget != null || (whoIsEnemy == Affiliation.Player && playerTarget != null); }
    
    protected virtual void GetCurrentTarget()
    {
        if(targetsInRange.Count <= 0){
            currentTarget = null;
            return;
        }
        
        if(currentTarget != null){
            currentTarget.onUnitDeath -= HandleTargetDeath;
        }
        
        currentTarget = targetingStyle switch
        {
            TargetingStyle.First => targetsInRange.First(),
            TargetingStyle.Last => targetsInRange.Last(),
            TargetingStyle.Strong => targetsInRange.OrderBy(t => t.health).First(),
            TargetingStyle.Weak => targetsInRange.OrderBy(t => t.health).Last(),
            
            _ => targetsInRange.First()
        };
        
        currentTarget.onUnitDeath += HandleTargetDeath;
    }
    
    protected virtual Vector3 GetFinalAttackDirection()
    {
        Vector3 attackDir = Vector3.zero;
        if(currentTarget != null){
            attackDir = currentTarget.transform.position - attackPoint.position;
        }
        else if (currentTarget == null && playerTarget != null){
            attackDir = playerTarget.transform.position - attackPoint.position;
        }
        attackDir.y += 1;
        
        return attackDir;
    }
    
    protected void HandleTargetDeath(Unit target){
        target.onUnitDeath -= HandleTargetDeath;
        RemoveTarget(target);
        GetCurrentTarget();
    }
    
    public void AddTarget(Unit target)
    {
        if(targetsInRange.Contains(target)) return;
        
        targetsInRange.Add(target);
        GetCurrentTarget();
    }
    public void RemoveTarget(Unit target)
    {
        if(!targetsInRange.Contains(target)) return;
        
        targetsInRange.Remove(target);
        GetCurrentTarget();
    }
}