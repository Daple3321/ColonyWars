using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class GameAssets
{
    public static GameObject playerPrefab;
    public static Controls controls;
    
    public static GameObject itemPrefab;
    public static List<ItemData> itemDatas;
    public static GameObject inventoryUI_Prefab;
    public static GameObject hotbarUI_Prefab;

    public static GameObject projectilePrefab;
    public static GameObject hitParts;

    private static EventBus eventBus;
    
    public static bool isInitialized { get; private set; } = false;

    public static void Init()
    {
        if (!isInitialized)
        {
            Stopwatch resLoad = Stopwatch.StartNew();
            
            eventBus = new EventBus();
            
            playerPrefab = Resources.Load<GameObject>("Player/Player");
            controls = null;
            controls = new Controls();
            
            itemDatas = Resources.LoadAll<ItemData>("Items/").ToList();
            itemPrefab = Resources.Load<GameObject>("ItemPrefab");

            inventoryUI_Prefab = Resources.Load<GameObject>("UI/Inventory");
            hotbarUI_Prefab = Resources.Load<GameObject>("UI/Hotbar");
            
            projectilePrefab = Resources.Load<GameObject>("Projectile");
            
            hitParts = Resources.Load<GameObject>("Effects/hitEffect_1");
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
