using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController i { get; private set; }

    public static Player p;
    public static WorldGenerator worldGenerator;
    public static ObjectGenerator objectGenerator;
    public static TimeManager timeManager;

    //public InventoryUI inventoryUI;

    public Canvas mainCanvas;
    public Canvas worldCanvas;
    public Canvas squadCanvas;
    public SquadPanel playerSquadPanel;
    public SquadAssemblePanel squadAssemblePanel;
    public MouseFollower mouseFollower;
    public Transform pSpawnPoint;
    public static Terrain currentTerrain;
    public GameSettings defaultGameSettings;
    
    private EventBus eventBus;
    
    void Awake()
    {
        if (i != null){
            Destroy(this);
        }
        else{
            i = this;
        }
        
        eventBus = new EventBus(); // reseting event bus
        //EventBus.i.PlayerStaminaChanged += (x, y) => Debug.Log($"{EventBus.i.PlayerStaminaChanged.GetInvocationList()}");
        ResetStaticVars();
    }

    public void ResetStaticVars()
    {
        p = null;
        worldGenerator = null;
        objectGenerator = null;
        timeManager = null;
    }

    // С параметрами старта (генерация мира, настройки, персонаж)
    public static event Action OnGameStarted;
    private void StartGame(GameSettings gameSettings = null)
    {
        ResetStaticVars();
        
        GameAssets.Init(); // это должно быть при запуске игры (в главном меню)
        if (gameSettings == null){
            gameSettings = defaultGameSettings;
        }

        worldGenerator = GameObject.Find("WorldGen").GetComponent<WorldGenerator>();
        worldGenerator.Init(gameSettings.worldGenSettings);
        currentTerrain = worldGenerator.terrain;
        
        objectGenerator = GameObject.Find("WorldGen").GetComponent<ObjectGenerator>();
        objectGenerator.Init(gameSettings.objectGenSettings);
        
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        timeManager.Init();
        
        //Pools.Init();

        //inventoryUI = GameObject.Find("PlayerInventory").GetComponent<InventoryUI>();
        mainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        worldCanvas = GameObject.Find("WorldCanvas").GetComponent<Canvas>();
        
        squadCanvas = GameObject.Find("SquadCanvas").GetComponent<Canvas>();
        playerSquadPanel = GameObject.Find("SquadPanel").GetComponent<SquadPanel>();
        squadAssemblePanel = GameObject.Find("SquadAssemblePanel").GetComponent<SquadAssemblePanel>();
        
        mouseFollower = GameObject.Find("MouseFollower").GetComponent<MouseFollower>();

        GameObject playerObj = Instantiate(GameAssets.playerPrefab, pSpawnPoint.position, Quaternion.identity);
        p = playerObj.GetComponent<Player>();
        p.InitPlayer();
        
        worldCanvas.worldCamera = Camera.main; // after player
        
        OnGameStarted?.Invoke();
        // World gen
        // Reference assigning
    }
    
    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            Helper.RestartCurrentScene();
        }
        if(Input.GetKeyDown(KeyCode.Keypad3))
        {
            objectGenerator.GenerateObjects().Forget();
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            worldGenerator.GenerateTerrain().Forget();
        }
    }


    public static Vector3 GetPointOnTerrain(Vector3 point)
    {
        return new Vector3(point.x, currentTerrain.SampleHeight(point), point.z);
    }
    public static float GetTerrainAngle(Vector3 point)
    {
        float angle = 0;
        
        Vector3 rayOrigin = new Vector3(point.x, 50, point.z);
        Ray ray = new Ray(rayOrigin, Vector3.down);
        RaycastHit[] hit = new RaycastHit[1];
        if(Physics.RaycastNonAlloc(ray, hit, Mathf.Infinity, LayerMask.GetMask("Ground")) > 0) // добавить ещё проверку на объекты вокруг? overlapSphere
        {
            angle = Vector3.Angle(Vector3.up, hit[0].normal);
        }
        
        return angle;
    }
}
