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
    public static GameObject worldSpaceBar;
    public static GameObject unitSlot_Prefab;
    public static GameObject nearbyUnitSlot_Prefab;
    
    // ----- Building -----
    public static GameObject buildCanvas;
    public static GameObject buildingSlot;
    public static GameObject buildingInventory;
    
    public static GameObject buildingPanel;
    public static GameObject buildingHoverPanel;
    public static GameObject minePanel;
    public static GameObject colonyCenterPanel;
    public static GameObject craftingStationPanel;
    public static GameObject barracksPanel;
    // --------------------
    
    public static GameObject craftSlot;
    public static GameObject unitCraftSlot;
    
    public static GameObject crosshairCanvas;

    public static GameObject projectilePrefab;
    public static GameObject groundHitParts;
    public static GameObject unitHitParts;
    public static GameObject stunEffect;

    //private static EventBus eventBus;
    
    public static Material intersectionMaterial;
    public static Mesh box;
    public static Mesh cylinder;
    public static Mesh sphere;
    public static Mesh capsule;
    
    public static GameObject objectPooler;
    
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
            worldSpaceBar = Resources.Load<GameObject>("UI/WorldSpaceBar");
            unitSlot_Prefab = Resources.Load<GameObject>("UI/UnitSlot");
            nearbyUnitSlot_Prefab = Resources.Load<GameObject>("UI/NearbyUnitSlot");
            //UnityEngine.Debug.Log(unitSlot_Prefab + "Loaded");
            
            buildingInventory = Resources.Load<GameObject>("UI/BuildingInventory");
            buildCanvas = Resources.Load<GameObject>("UI/BuildCanvas");
            buildingSlot = Resources.Load<GameObject>("UI/BuildingSlot");
            buildingPanel = Resources.Load<GameObject>("UI/BuildingPanel");
            buildingHoverPanel = Resources.Load<GameObject>("UI/BuildingHoverCanvas");
            minePanel = Resources.Load<GameObject>("UI/MinePanel");
            colonyCenterPanel = Resources.Load<GameObject>("UI/ColonyCenterPanel");
            craftingStationPanel = Resources.Load<GameObject>("UI/Crafting/CraftingStationPanel");
            barracksPanel = Resources.Load<GameObject>("UI/Crafting/BarracksPanel");
            
            craftSlot = Resources.Load<GameObject>("UI/Crafting/CraftSlot");
            unitCraftSlot = Resources.Load<GameObject>("UI/Crafting/UnitCraftSlot");
            
            crosshairCanvas = Resources.Load<GameObject>("UI/CrosshairCanvas");
            
            projectilePrefab = Resources.Load<GameObject>("Projectile");
            
            groundHitParts = Resources.Load<GameObject>("Effects/hitEffect_1");
            unitHitParts = Resources.Load<GameObject>("Effects/bloodHit_1");
            stunEffect = Resources.Load<GameObject>("Effects/StunEffect");
            
            objectPooler = Resources.Load<GameObject>("ObjectPooler");
            
            intersectionMaterial = Resources.Load<Material>("IntersectionMat");
            
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
