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
    public static GameObject worldBar;
    public static GameObject unitSlot_Prefab;
    public static GameObject nearbyUnitSlot_Prefab;

    public static GameObject projectilePrefab;
    public static GameObject groundHitParts;
    public static GameObject unitHitParts;

    //private static EventBus eventBus;
    
    public static Colors colors;
    
    public static bool isInitialized { get; private set; } = false;

    public static void Init()
    {
        //eventBus = new EventBus();
        if (!isInitialized)
        {
            Stopwatch resLoad = Stopwatch.StartNew();
            
            
            playerPrefab = Resources.Load<GameObject>("Player/Player");
            controls = null;
            controls = new Controls();
            
            itemDatas = Resources.LoadAll<ItemData>("Items/").ToList();
            itemPrefab = Resources.Load<GameObject>("ItemPrefab");

            inventoryUI_Prefab = Resources.Load<GameObject>("UI/Inventory");
            hotbarUI_Prefab = Resources.Load<GameObject>("UI/Hotbar");
            worldBar = Resources.Load<GameObject>("UI/WorldBar");
            unitSlot_Prefab = Resources.Load<GameObject>("UI/UnitSlot");
            nearbyUnitSlot_Prefab = Resources.Load<GameObject>("UI/NearbyUnitSlot");
            //UnityEngine.Debug.Log(unitSlot_Prefab + "Loaded");
            
            projectilePrefab = Resources.Load<GameObject>("Projectile");
            
            groundHitParts = Resources.Load<GameObject>("Effects/hitEffect_1");
            unitHitParts = Resources.Load<GameObject>("Effects/bloodHit_1");
            
            colors = Resources.Load<Colors>("ColorPreset");
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
