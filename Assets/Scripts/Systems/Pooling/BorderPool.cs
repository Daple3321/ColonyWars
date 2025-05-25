using UnityEngine;
using UnityEngine.Pool;

public class BorderPool : MonoBehaviour
{
    public PoolType poolType;

    // Collection checks will throw errors if we try to release an item that is already in the pool.
    public bool collectionChecks = true;
    public int maxPoolSize = 10;

    IObjectPool<GameObject> m_Pool;

    public IObjectPool<GameObject> Pool
    {
        get
        {
            if (m_Pool == null)
            {
                if (poolType == PoolType.Stack)
                    m_Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
                else
                    m_Pool = new LinkedPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, maxPoolSize);
            }
            return m_Pool;
        }
    }
    
    public GameObject CreatePooledItem()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = "Pooled border";
        go.isStatic = true;
        Destroy(go.GetComponent<Collider>());
        go.GetComponent<Renderer>().material = GameAssets.intersectionMaterial;
        
        return go;
    }
    
    // Called when an item is returned to the pool using Release
    void OnReturnedToPool(GameObject system)
    {
        system.gameObject.SetActive(false);
    }
    
    // Called when an item is taken from the pool using Get
    void OnTakeFromPool(GameObject system)
    {
        system.gameObject.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    void OnDestroyPoolObject(GameObject system)
    {
        Destroy(system.gameObject);
    }
}
