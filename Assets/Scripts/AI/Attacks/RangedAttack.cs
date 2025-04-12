using UnityEngine;

[System.Serializable]
public class RangedAttack : Attack
{
    public RangedAttackData rangedAttackData;
    public RangedAttack(Unit owner, AttackData attackData, Affiliation affiliation) : base(owner, attackData, affiliation){
        if(attackData is RangedAttackData rangedAttackData){
            this.rangedAttackData = rangedAttackData;
        }
    }

    public override void ConstantAttack()
    {
        //Debug.Log($"Constant attack from: {owner.gameObject.name}");
    }

    public override void OneShotAttack()
    {
        Vector3 shootDir = owner.attackTarget.position - owner.attackPoint.position;
        shootDir.y += 1f;
        
        float scatterAmount = Random.Range(rangedAttackData.projectileScatter.x, rangedAttackData.projectileScatter.y);
        Ray shootRay = new Ray(owner.attackPoint.position, Helper.GetRandPointOnUnitSphereCap(shootDir, scatterAmount));
        
        Projectile projectile = GameObject.Instantiate(rangedAttackData.projectilePrefab, owner.attackPoint.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.transform.up = shootRay.direction;
        projectile.Init(owner.damage, 
            rangedAttackData.projectileSpeed, 
            affiliation, 
            rangedAttackData.penetrationAmount, 
            rangedAttackData.projectileLifetime
        );

        Debug.DrawRay(owner.attackPoint.position, shootRay.direction * 4, Color.cyan, 2);
        //Debug.Log($"OneShot attack from: {owner.gameObject.name}");
    }
}
