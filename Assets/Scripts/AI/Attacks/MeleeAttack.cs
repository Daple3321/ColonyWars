using UnityEngine;

public class MeleeAttack : Attack
{
    public MeleeAttackData meleeAttackData;
    public MeleeAttack(Unit owner, AttackData attackData, Affiliation affiliation) : base(owner, attackData, affiliation){
        if(attackData is MeleeAttackData meleeAttackData){
            this.meleeAttackData = meleeAttackData;
        }
    }

    public override void ConstantAttack()
    {
        
    }

    public override void OneShotAttack()
    {
        Vector3 shootDir = owner.attackTarget.position - owner.attackPoint.position;
        shootDir.y += 1;
        
        //float scatterAmount = Random.Range(rangedAttackData.projectileScatter.x, rangedAttackData.projectileScatter.y);
        Ray shootRay = new Ray(owner.attackPoint.position, shootDir);
        
        RaycastHit hit;
        if (Physics.Raycast(shootRay, out hit, owner.attackDistance, owner.attackHitMask))
        {
            IDamageable damageable;
            if (hit.collider.gameObject.TryGetComponent<IDamageable>(out damageable))
            {
                damageable.TakeDamage(owner.damage);
            }
            Helper.SpawnHitEffect(hit.point, hit.normal, hit.collider.gameObject.layer);
            
            //GameObject lineObj = Instantiate(hitScanLine, shootPoint.position, Quaternion.FromToRotation(Vector3.zero, hit.point - shootPoint.position));
            //lineObj.GetComponent<LineRenderer>().SetPosition(1, hit.point);
            
            //Debug.Log($"Hit {hit.collider.name}");
            Debug.DrawLine(owner.attackPoint.position, hit.point, Color.yellow, 3);
        }
    }
}
