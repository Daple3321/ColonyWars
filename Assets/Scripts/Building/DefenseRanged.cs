using System.Collections;
using UnityEditor;
using UnityEngine;
using static EntityStatType;

public class DefenseRanged : Defense
{
    [Header("Attack Settings")]
    [SerializeReference, SubclassSelector] public AttackData attackData;
    [SerializeReference] public TowerAttack currentAttack;
    
    protected float _attackSpeed;
    public bool isAttacking = false;
    
    protected override void Init()
    {
        base.Init();
        
        _attackSpeed = stats[attackSpeed].Value;
        
        if(attackData is RangedAttackData){
            currentAttack = new RangedTowerAttack(this, attackData, affiliation);
        }
        else if(attackData is HitscanAttackData){
            currentAttack = new HitscanTowerAttack(this, attackData, affiliation);
        }
    }
    
    void Update()
    {
        HandleAttacking();
    }

    public override bool CanAttack()
    {
        return base.CanAttack() && _attackSpeed <= 0f && !isAttacking;
    }
    protected virtual void HandleAttacking() 
    {
        if(_attackSpeed > 0){
            _attackSpeed -= Time.deltaTime;
        }
        
        if(CanAttack()){
            StartCoroutine(Attack());
        }
    }
    
    public virtual IEnumerator Attack()
    {
        isAttacking = true;
        currentAttack.attackFinished = false;
        
        
        currentAttack.OneShotAttack(GetFinalAttackDirection());

        while(!currentAttack.attackFinished)
        {
            currentAttack.ConstantAttack(GetFinalAttackDirection());
            yield return null;
        }
        
        _attackSpeed = stats[attackSpeed].Value;
        while(_attackSpeed > 0)
        {
            _attackSpeed -= Time.deltaTime;
            yield return null;
        }
        
        isAttacking = false;
    }
    
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        //Vector3 labelPos2 = new Vector3(transform.position.x + 0.5f, transform.position.y+1, transform.position.z + stats[attackDistance].Value+ 0.1f);
        //Handles.Label(labelPos2, "Detection range");
        Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.up, stats[attackDistance].Value);
    }

#endif
}
