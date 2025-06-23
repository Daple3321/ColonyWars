using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static EntityStatType;

public class MeleeAttack : Attack
{
    public MeleeAttackData meleeAttackData;
    public MeleeAttack(Unit owner, AttackData attackData, Affiliation affiliation) : base(owner, attackData, affiliation){
        if(attackData is MeleeAttackData meleeAttackData){
            this.meleeAttackData = meleeAttackData;
            this.attackSequence = meleeAttackData.attackSequence;
            attackSequence.Init();
            owner.combat.stats[damage].AddModifier(attackSequence.damageMod);
        }
    }
    
    public AttackSequence attackSequence;
    private float _attackCd;
    public override void ConstantAttack()
    {
        if (_attackCd > 0){
            _attackCd -= Time.deltaTime;
        }
        else{
            HandleAttack();
        }
    }

    public override void OneShotAttack(){
        
    }
    
    public void HandleAttack()
    {
        _attackCd = attackSequence.CurrentAttack().duration;
        owner.combat.stats[damage].OnModifierChanged();
        //owner.stats[runSpeed].OnModifierChanged();
        
        switch(meleeAttackData.meleeAttackType){
            case MeleeAttackType.RAYCAST:
                PerformRaycastAttack();
            break;
            
            case MeleeAttackType.BOX:
                PerformBoxcastAttack();
            break;
            
            case MeleeAttackType.SPHERE:
                
            break;
        }
        
        if(!attackSequence.Attack()){ // если дошли до конца
            attackFinished = true;   
        }
    }
    public async void PerformBoxcastAttack()
    {
        Vector3 boxPos = owner.attackPoint.position + (owner.attackPoint.forward * (meleeAttackData.attackBoxExtents.z/2));
        Vector3 boxSize = meleeAttackData.attackBoxExtents;
        Collider[] hits = Physics.OverlapBox(boxPos, boxSize, owner.attackPoint.rotation, owner.attackHitMask);
        
        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // go.transform.position = boxPos;
        // go.transform.localScale = boxSize;
        // go.transform.rotation = owner.attackPoint.rotation;
        // GameObject.Destroy(go.GetComponent<Collider>());
        // GameObject.Destroy(go, 0.35f);
        if (hits.Length > 0)
        {
            IDamageable damageable;
            foreach(Collider hit in hits)
            {
                if (hit.gameObject.TryGetComponent(out damageable)){
                    var result = await damageable.TakeDamage(owner.combat.stats[damage].Value, owner, -hit.transform.forward*meleeAttackData.knockBackForce);
                    HandleAttackResult(result, hit);
                }
            }
        }
    }
    public async void PerformRaycastAttack()
    {
        Vector3 shootDir = owner.attackTarget.position - owner.attackPoint.position;
        shootDir.y += 1;
        
        //float scatterAmount = Random.Range(rangedAttackData.projectileScatter.x, rangedAttackData.projectileScatter.y);
        Ray shootRay = new Ray(owner.attackPoint.position, shootDir);
        
        RaycastHit hit;
        if (Physics.Raycast(shootRay, out hit, owner.combat.stats[attackDistance].Value, owner.attackHitMask))
        {
            IDamageable damageable;
            if (hit.collider.gameObject.TryGetComponent(out damageable))
            {
                var result = await damageable.TakeDamage(owner.combat.stats[damage].Value, owner, shootRay.direction);
                HandleAttackResult(result, hit.collider);
            }
            //Helper.SpawnHitEffect(hit.point, hit.normal, hit.collider.gameObject.layer);

            Debug.DrawLine(owner.attackPoint.position, hit.point, Color.yellow, 3);
        }
    }
    
    private void HandleAttackResult(DamageResult result, Collider hit)
    {
        switch(result){
        case DamageResult.Dealt:
            Helper.SpawnHitEffect(hit.transform.position, -hit.transform.forward, hit.gameObject.layer);
        break;
        
        case DamageResult.Blocked:
            owner.stun.AddStun(2f);
            
        break;
        
        case DamageResult.Missed:
            TextMeshProUGUI t = WorldUI.i.SpawnPopup(hit.transform.position, Color.white, Vector3.one, 0.4f);
            t.text = "Missed!";
        break;
        
        case DamageResult.Killed:
            
        break;
        }
    }

    public override void OnAttackCanceled(){
        attackSequence.ResetSequence();
    }
}
