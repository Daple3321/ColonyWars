using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public Explosion explosion;
    
    public float explosionDelay;
    
    public CinemachineImpulseSource impulseSource;
    
    public void Init(float explosionDelay = 3f)
    {
        this.explosionDelay = explosionDelay;
    }

    void Start()
    {
        StartCoroutine(StartTimer());
    }

    public IEnumerator StartTimer(){
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }
    
    public virtual void Explode()
    {
        explosion.Explode(transform.position);
        
        impulseSource.GenerateImpulse();
        
        Destroy(gameObject);
    }
}
