using Unity.Cinemachine;
using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    public Explosion explosion;
    public CinemachineImpulseSource impulseSource;
    
    public override void Init(float damage, float speed, Affiliation affiliation, 
        GameObject owner = null, int penetrationAmount = 0, float lifeTime = 2.5f, float knockBackForce = 0f,
        DamageType damageType = DamageType.Ranged)
    {
       base.Init(damage, speed, affiliation, owner, penetrationAmount, lifeTime, knockBackForce, damageType);
       
       explosion.damage = damage;
       explosion.knockBackForce = knockBackForce;
       
       Vector3 moveDir = transform.up * speed * Time.deltaTime;
        rb.AddForce(moveDir, ForceMode.Impulse);
    }
    
    protected override void Update()
    {
        
    }
    protected void FixedUpdate()
    {
        // Vector3 moveDir = transform.right * speed * Time.deltaTime;
        // rb.AddForce(moveDir);
    }

    protected override void ProccessHit(HitType hitType)
    {
        if (penetrationAmount > 0)
        {
            penetrationAmount--;
            SpawnExplosion();
            SpawnEffects(hitType);
        }
        else if(penetrationAmount <= 0)
        {
            DestroyWithEffects(hitType);
        }
    }
    protected override void DestroyWithEffects(HitType hitType)
    {
        SpawnExplosion();
        SpawnEffects(hitType);
        Destroy(gameObject);
    }
    
    protected void SpawnExplosion()
    {
        explosion.Explode(transform.position, owner);
        impulseSource.GenerateImpulse();
    }
    
    protected override void OnTriggerEnter(Collider col)
    {            
        if (col.gameObject.layer == 7) // ground
        {
            DestroyWithEffects(HitType.GROUND);
        }
        if (col.gameObject.layer == 6) // player
        {
            //Debug.Log("Player hit");
            ProccessHit(HitType.PLAYER);
        }
        if (col.gameObject.layer == 11) // enemy unit
        {
            //Debug.Log("Enemy unit hit");
            ProccessHit(HitType.UNIT);
        }
        if (col.gameObject.layer == 12) // player unit
        {
            //Debug.Log("Player unit hit");
            ProccessHit(HitType.UNIT);
        }
        if (col.gameObject.layer == 13 || col.gameObject.layer == 14) // any building
        {
            //Debug.Log("Building hit");
            ProccessHit(HitType.BUILDING);
        }
        //Debug.Log($"Hit {col.gameObject.name}");
    }
}
