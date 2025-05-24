using Unity.Cinemachine;
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

    [SerializeField] private Rigidbody rb;
    [SerializeField] private TrailRenderer trail;
    //[SerializeField] private Material mat;
    [SerializeField] private Renderer rend;
    private MaterialPropertyBlock propertyBlock;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    
    public virtual void Init(float damage, float speed, Affiliation affiliation, int penetrationAmount = 0, float lifeTime = 2.5f, float knockBackForce = 0f)
    {
        propertyBlock = new MaterialPropertyBlock();
        //rb = GetComponent<Rigidbody>();
        //rend = GetComponent<Renderer>();
        //trail = GetComponentInChildren<TrailRenderer>();
        //mat = GetComponent<Renderer>().material;
        
        this.damage = damage;
        this.speed = speed;
        this.penetrationAmount = penetrationAmount;
        this.knockBackForce = knockBackForce;
        this.lifeTime = lifeTime;
        
        this.affiliation = affiliation;
        SetupAffiliation();
        
        //impulseSource.GenerateImpulse(Camera.main.transform.forward);

        Destroy(gameObject, lifeTime);
    }
    
    void SetupAffiliation()
    {
        if(affiliation == Affiliation.Enemy){
            currentExcludeMask = enemyExcludeMask;
            gameObject.layer = 9;
            propertyBlock.SetColor("_Color", Color.red);
            rend.SetPropertyBlock(propertyBlock);
            //mat.color = Color.red;
            
            trail.colorGradient = GameAssets.colors.enemyBulletTrail;
        }
        else if(affiliation == Affiliation.Player){
            currentExcludeMask = playerExcludeMask;
            gameObject.layer = 10;
            propertyBlock.SetColor("_Color", Color.yellow);
            rend.SetPropertyBlock(propertyBlock);
            //mat.color = Color.yellow;
            
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
            hit = PoolManager.Get(GameAssets.unitHitParts);
            hit.transform.position = transform.position;
            hit.transform.rotation = Quaternion.LookRotation(-transform.up, Vector3.right);
            //hit = Instantiate(GameAssets.unitHitParts, transform.position, Quaternion.LookRotation(-transform.up, Vector3.right));
                break;
            
            case HitType.PLAYER:
            hit = PoolManager.Get(GameAssets.unitHitParts);
            hit.transform.position = transform.position;
            hit.transform.rotation = Quaternion.LookRotation(-transform.up, Vector3.right);
            //hit = Instantiate(GameAssets.unitHitParts, transform.position, Quaternion.LookRotation(-transform.up, Vector3.right));
                break;
            
            case HitType.GROUND:
            hit = PoolManager.Get(GameAssets.groundHitParts);
            hit.transform.position = transform.position;
            hit.transform.rotation = Quaternion.LookRotation(-transform.up, Vector3.right);
            //hit = Instantiate(GameAssets.groundHitParts, transform.position, Quaternion.LookRotation(-transform.up, Vector3.right));
                break;
            
            case HitType.BUILDING:
            hit = PoolManager.Get(GameAssets.groundHitParts);
            hit.transform.position = transform.position;
            hit.transform.rotation = Quaternion.LookRotation(-transform.up, Vector3.right);
            //hit = Instantiate(GameAssets.groundHitParts, transform.position, Quaternion.LookRotation(-transform.up, Vector3.right));
                break;
        }
        
        //Destroy(hit, 5);
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
            damageable.TakeDamage(damage, transform.up.normalized*knockBackForce);
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