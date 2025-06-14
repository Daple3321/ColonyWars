using System.Collections;
using UnityEditor;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeReference] public Attack currentAttack;
    [SerializeReference, SubclassSelector] public AttackData attackData;
    
    [Space(10)]
    public float damage;
    public float attackDistance = 1f;
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
    public virtual bool IsAttacking() { return false;}
    
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Vector3 labelPos2 = new Vector3(transform.position.x + 0.5f, transform.position.y+1, transform.position.z + attackDistance + 0.1f);
        Handles.Label(labelPos2, "Attack distance");
        Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.up, attackDistance);
    }

#endif
}
