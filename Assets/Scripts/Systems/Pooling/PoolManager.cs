using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public static class PoolManager
{
    private static Dictionary<GameObject, ObjectPool<GameObject>> _pools = new();
    
    public static void Init(){
        _pools.Clear();
        _pools = new();
    }
    
    public static GameObject Get(GameObject prefab)
    {
        if (!_pools.ContainsKey(prefab))
        {
            CreatePool(prefab);
        }
        return _pools[prefab].Get();
    }
    
    private static void CreatePool(GameObject prefab, int defaultCapacity = 10, int maxSize = 20)
    {
        var pool = new ObjectPool<GameObject>(
            createFunc: () => InstantiatePrefab(prefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => OnDestroyPoolObject(obj),
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        _pools[prefab] = pool;
    }

    static void OnDestroyPoolObject(GameObject system)
    {
        Object.Destroy(system.gameObject);
    }
    
    private static GameObject InstantiatePrefab(GameObject prefab)
    {
        var instance = Object.Instantiate(prefab);
        var returnToPool = instance.AddComponent<ReturnToPool>();
        returnToPool.Prefab = prefab; // Запоминаем префаб для возврата
        return instance;
    }

    public static void Release(GameObject instance)
    {
        var returnToPool = instance.GetComponent<ReturnToPool>();
        if (returnToPool != null && returnToPool.Prefab != null)
        {
            _pools[returnToPool.Prefab].Release(instance);
        }
    }
}
