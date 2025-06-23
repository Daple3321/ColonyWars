using System.Collections;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;
using static EntityStatType;

public class UnitCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeReference] public Attack currentAttack;
    [SerializeReference, SubclassSelector] public AttackData attackData;
    
    [Space(10)]
    [SerializedDictionary("Stat Type", "Unit Stat")]
    public SerializedDictionary<EntityStatType, Stat> stats;
    //public float damage;
    //public float attackDistance = 1f;
    public float attackDuration;
    public float attackSpeed;
    protected float _attackSpeed;
    public float concentration;
    public bool isAttacking;
    
    private Unit owner;

    void Awake(){
        enabled = false;
    }

    public void Init(Unit owner){
        this.owner = owner;
        enabled = true;
    }
    
    protected Coroutine attackRoutine;
    public virtual void HandleAttacking() 
    {
        if(CanAttack())
        {
            attackRoutine = StartCoroutine(Attack());
        }
    }
    
    public void CancelAttackDelayed(){
        if(cancelRoutine == null){
            cancelRoutine = StartCoroutine(CancelRoutine());
        }
    }
    public void CancelAttackInstant(){
        if(attackRoutine != null){
            StopCoroutine(attackRoutine);
        }
        currentAttack.OnAttackCanceled();
        isAttacking = false;
        currentAttack.attackFinished = true;
    }
    private Coroutine cancelRoutine;
    private IEnumerator CancelRoutine()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("Attack canceled!");
        StopCoroutine(attackRoutine);
        currentAttack.OnAttackCanceled();
        isAttacking = false;
        currentAttack.attackFinished = true;
        
        cancelRoutine = null;
    }
    public virtual IEnumerator Attack()
    {
        isAttacking = true;
        currentAttack.attackFinished = false;
        
        currentAttack.OneShotAttack();
        // Attack effects&animations here

        //float attackDur = this.attackDuration;
        while(!currentAttack.attackFinished)
        {
            currentAttack.ConstantAttack();
            //attackDur -= Time.deltaTime;
            yield return null;
        }
        
        _attackSpeed = this.attackSpeed;
        while(_attackSpeed > 0)
        {
            _attackSpeed -= Time.deltaTime;
            yield return null;
        }
        
        isAttacking = false;
        //Debug.Log($"[{owner.unitName}] Attack ended!", owner.gameObject);
    }
    public virtual bool CanAttack(){ return isAttacking ? false : true && _attackSpeed <= 0; }
    //public virtual bool IsAttacking() { return false;}
    
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Vector3 labelPos2 = new Vector3(transform.position.x + 0.5f, transform.position.y+1, transform.position.z + stats[attackDistance].Value+ 0.1f);
        Handles.Label(labelPos2, "Attack distance");
        Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.up, stats[attackDistance].Value);
    }

#endif
}
