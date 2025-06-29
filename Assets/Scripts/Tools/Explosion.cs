using UnityEngine;

[System.Serializable]
public class Explosion
{
    public float damage;
    public float radius;
    public float knockBackForce;
    public AnimationCurve damageDissipation;
    
    public LayerMask hitMask;
    
    public Explosion(float damage, float radius, float knockBackForce, LayerMask hitMask){
        this.damage = damage;
        this.radius = radius;
        this.knockBackForce = knockBackForce;
        this.hitMask = hitMask;
    }
    
    public void Explode(Vector3 pos)
    {
        Collider[] hits = Physics.OverlapSphere(pos, radius, hitMask);
        
        if (hits.Length > 0)
        {
            IDamageable damageable;
            foreach(Collider hit in hits)
            {
                if (hit.gameObject.TryGetComponent(out damageable))
                {
                    Vector3 knockback = (hit.transform.position - pos).normalized;
                    float distance = Vector3.Distance(pos, hit.transform.position);
                    float distanceLerp = Mathf.InverseLerp(0, radius, distance);
                    float finalDamage = 1 + (damage * damageDissipation.Evaluate(distanceLerp));
                    damageable.TakeDamage(finalDamage, this, knockback*knockBackForce, DamageType.Explosive);
                }
            }
        }
        
        GameObject explosion = GameObject.Instantiate(GameAssets.explosion, pos, Quaternion.identity);
        GameObject.Destroy(explosion.gameObject, 7f);
    }
}
