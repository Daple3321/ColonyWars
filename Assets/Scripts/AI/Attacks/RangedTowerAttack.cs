using UnityEngine;
using static EntityStatType;

public class RangedTowerAttack : TowerAttack
{
    public RangedAttackData rangedAttackData;
    public RangedTowerAttack(Defense owner, AttackData attackData, Affiliation affiliation) : base(owner, attackData, affiliation){
        if(attackData is RangedAttackData rangedAttackData){
            this.rangedAttackData = rangedAttackData;
        }
    }

    public override void OneShotAttack(Vector3 attackDir)
    {
        // Vector3 shootDir = owner.currentTarget.transform.position - owner.attackPoint.position;
        // shootDir.y += 1f; // поправка на высоту цели
        
        for(int i = 0; i < rangedAttackData.bulletsPerShot; i++)
        {
            float scatterAmount = Random.Range(rangedAttackData.projectileScatter.x, rangedAttackData.projectileScatter.y);
            Ray shootRay = new Ray(owner.attackPoint.position, Helper.GetRandPointOnUnitSphereCap(attackDir, scatterAmount));
            
            Projectile projectile = GameObject.Instantiate(rangedAttackData.projectilePrefab, owner.attackPoint.position, Quaternion.identity).GetComponent<Projectile>();
            projectile.transform.up = shootRay.direction;
            projectile.Init(
                owner.stats[damage].Value, 
                rangedAttackData.projectileSpeed, 
                affiliation,
                owner.gameObject, 
                rangedAttackData.penetrationAmount, 
                rangedAttackData.projectileLifetime,
                rangedAttackData.knockBackForce,
                rangedAttackData.damageType
            );

            Debug.DrawRay(owner.attackPoint.position, shootRay.direction * 4, Color.cyan, 2);
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
