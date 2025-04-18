using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float knockBackForce;
    public float speed;
    public int penetrationAmount;
    public float lifeTime;
    public Affiliation affiliation;
    public LayerMask currentExcludeMask;
    public LayerMask playerExcludeMask;
    public LayerMask enemyExcludeMask;

    private Rigidbody rb;
    private TrailRenderer trail;

    public virtual void Init(float damage, float speed, Affiliation affiliation, int penetrationAmount = 0, float lifeTime = 2.5f)
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();
        
        this.damage = damage;
        this.speed = speed;
        this.penetrationAmount = penetrationAmount;
        this.lifeTime = lifeTime;
        
        this.affiliation = affiliation;
        SetupAffiliation();

        Destroy(gameObject, lifeTime);
    }
    
    void SetupAffiliation()
    {
        if(affiliation == Affiliation.Enemy){
            currentExcludeMask = enemyExcludeMask;
            gameObject.layer = 9;
            
            trail.colorGradient = GameAssets.colors.enemyBulletTrail;
        }
        else if(affiliation == Affiliation.Player){
            currentExcludeMask = playerExcludeMask;
            gameObject.layer = 10;
            
            trail.colorGradient = GameAssets.colors.playerBulletTrail;
        }
        rb.excludeLayers = currentExcludeMask;
    }

    void Update()
    {
        Vector3 moveDir = Vector3.up * speed * Time.deltaTime;
        transform.Translate(moveDir);
    }

    protected virtual void SpawnEffects(HitType hitType)
    {
        GameObject hit = null;
        switch (hitType)
        {
            case HitType.UNIT:
            hit = Instantiate(GameAssets.unitHitParts, transform.position, Quaternion.LookRotation(-transform.up, Vector3.right));
                break;
            
            case HitType.PLAYER:
            hit = Instantiate(GameAssets.unitHitParts, transform.position, Quaternion.LookRotation(-transform.up, Vector3.right));
                break;
            
            case HitType.GROUND:
            hit = Instantiate(GameAssets.groundHitParts, transform.position, Quaternion.LookRotation(-transform.up, Vector3.right));
                break;
            
            case HitType.BUILDING:
            hit = Instantiate(GameAssets.groundHitParts, transform.position, Quaternion.LookRotation(-transform.up, Vector3.right));
                break;
        }
        
        Destroy(hit, 5);
    }

    protected virtual void DestroyWithEffects(HitType hitType)
    {
        SpawnEffects(hitType);
        Destroy(gameObject);
    }

    protected virtual void ProccessHit(HitType hitType)
    {
        if (penetrationAmount > 0)
        {
            penetrationAmount--;
            SpawnEffects(hitType);
        }
        else if(penetrationAmount <= 0)
        {
            DestroyWithEffects(hitType);
        }
    }

    protected void OnTriggerEnter(Collider col)
    {
        IDamageable damageable;
        if (col.gameObject.TryGetComponent<IDamageable>(out damageable))
        {
            damageable.TakeDamage(damage, knockBackForce);
        }
            
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
        //Debug.Log($"Hit {col.gameObject.name}");
    }
}

public enum Affiliation : byte
{
    None,
    Enemy,
    Player,
}

public enum HitType : byte
{
    NONE,
    GROUND,
    UNIT,
    PLAYER,
    BUILDING,
}