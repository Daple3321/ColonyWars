using UnityEngine;
using UnityEngine.Pool;

public class ReturnToPool : MonoBehaviour
{
    public GameObject Prefab { get; set; }
    public ParticleSystem system;
    //public IObjectPool<ParticleSystem> pool;

    void Start()
    {
        //system = GetComponent<ParticleSystem>();
        if(TryGetComponent<ParticleSystem>(out system)){
            ParticleSystem.MainModule main = system.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }
    }

    void OnParticleSystemStopped()
    {
        // Return to the pool
        //pool.Release(system);
        PoolManager.Release(gameObject);
    }
    
    
}
