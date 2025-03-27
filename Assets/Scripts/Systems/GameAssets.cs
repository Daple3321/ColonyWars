using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public static class GameAssets
{
    public static GameObject playerPrefab;
    public static Controls controls;
    
    public static GameObject itemPrefab;
    public static List<ItemData> itemDatas;

    public static GameObject projectilePrefab;

    public static bool isInitialized { get; private set; } = false;

    public static void Init()
    {
        if (!isInitialized)
        {
            Stopwatch resLoad = Stopwatch.StartNew();

            playerPrefab = Resources.Load<GameObject>("Player/Player");
            controls = null;
            controls = new Controls();
            
            itemDatas = Resources.LoadAll<ItemData>("Items/").ToList();
            itemPrefab = Resources.Load<GameObject>("ItemPrefab");
            
            projectilePrefab = Resources.Load<GameObject>("Projectile");
            // foreach (ItemData item in itemDatas)
            // {
            //     UnityEngine.Debug.Log($"Found item: {item.itemName}");
            // }

            isInitialized = true;
            resLoad.Stop();
            UnityEngine.Debug.Log($"Resources loaded. Loading time: {resLoad.ElapsedMilliseconds}ms");
        }
        else{
            UnityEngine.Debug.Log("Assets already initialized.");
        }
    }
}
