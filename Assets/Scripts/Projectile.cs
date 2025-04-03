using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float knockBackForce;
    public float speed;
    public int penetrationAmount;
    public float lifeTime;
    public Affiliation affiliation;
    public LayerMask collisionLayermask;

    public GameObject hitEffect;

    public virtual void Init(float damage, float speed, Affiliation affiliation)
    {
        this.damage = damage;
        this.speed = speed;
        this.affiliation = affiliation;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        Vector3 moveDir = Vector3.up * speed * Time.deltaTime;
        transform.Translate(moveDir);
    }

    protected virtual void SpawnEffects()
    {
        GameObject hit = Instantiate(hitEffect, transform.position, Quaternion.LookRotation(-transform.up, Vector3.right));
        Destroy(hit, 5);
    }

    protected virtual void DestroyWithEffects()
    {
        SpawnEffects();
        Destroy(gameObject);
    }

    protected virtual void ProccessHit()
    {
        if (penetrationAmount > 0)
        {
            penetrationAmount--;
            SpawnEffects();
        }
        else if(penetrationAmount <= 0)
        {
            DestroyWithEffects();
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
            DestroyWithEffects();
        }
        if (col.gameObject.layer == 11) // enemy
        {

            ProccessHit();
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