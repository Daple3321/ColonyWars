using UnityEngine;
using static EntityStatType;

public class HitscanTowerAttack : TowerAttack
{   
    public HitscanAttackData hitscanAttackData;
    public HitscanTowerAttack(Defense owner, AttackData attackData, Affiliation affiliation) : base(owner, attackData, affiliation)
    {
        if(attackData is HitscanAttackData hitscanAttackData){
            this.hitscanAttackData = hitscanAttackData;
        }
    }

    public override void OneShotAttack(Vector3 attackDir)
    {
        for(int i = 0; i < hitscanAttackData.projectilesPerShot; i++)
        {
            float scatterAmount = Random.Range(hitscanAttackData.projectileScatter.x, hitscanAttackData.projectileScatter.y);
            Ray shootRay = new Ray(owner.attackPoint.position, Helper.GetRandPointOnUnitSphereCap(attackDir, scatterAmount));
            
            RaycastHit hit;
            if (Physics.Raycast(shootRay, out hit, owner.stats[attackDistance].Value, owner.attackHitMask))
            {
                IDamageable damageable;
                if (hit.collider.gameObject.TryGetComponent(out damageable))
                {
                    var result = damageable.TakeDamage(owner.stats[damage].Value, owner.gameObject, shootRay.direction, DamageType.Ranged);
                }
                
                GameObject lineObj = GameObject.Instantiate(GameAssets.hitScanLine, owner.attackPoint.position, Quaternion.identity);
                lineObj.GetComponent<HitscanLine>().Init(owner.attackPoint.position, hit.point);
                Helper.SpawnHitEffect(hit.point, hit.normal, hit.collider.gameObject.layer);

                Debug.DrawLine(owner.attackPoint.position, hit.point, Color.yellow, 3);
            }
        }
    }
    public override void ConstantAttack(Vector3 attackDir)
    {
        attackFinished = true;
    }

    public override void OnAttackCanceled()
    {
        attackFinished = true;
    }

    public override void OnAttackEnded()
    {
        attackFinished = true;
    }


    public override void PerformAttack()
    {
        
    }
}
