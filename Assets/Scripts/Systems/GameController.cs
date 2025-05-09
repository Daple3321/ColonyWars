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
    public BuildingPanelManager buildingPanelManager;
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
        PoolManager.Init();
    }

    // С параметрами старта (генерация мира, настройки, персонаж)
    public static event Action OnGameStarted;
    private void StartGame(GameSettings gameSettings = null)
    {
        Application.targetFrameRate = 120;
        
        ResetStaticVars();
        
        GameAssets.Init(); // это должно быть при запуске игры (в главном меню)
        if (gameSettings == null){
            gameSettings = defaultGameSettings;
        }

        FindReferences();

        worldGenerator.Init(gameSettings.worldGenSettings);
        currentTerrain = worldGenerator.terrain;
        objectGenerator.Init(gameSettings.objectGenSettings);
        timeManager.Init();
        //Pools.Init();

        //inventoryUI = GameObject.Find("PlayerInventory").GetComponent<InventoryUI>();

        GameObject playerObj = Instantiate(GameAssets.playerPrefab, pSpawnPoint.position, Quaternion.identity);
        p = playerObj.GetComponent<Player>();
        p.InitPlayer();
        
        worldCanvas.worldCamera = Camera.main; // after player
        
        ColoniesManager.i.Init();
        ColoniesManager.i.SpawnEnemyColonies(gameSettings);
        
        PopUpManager.i.Init();
        
        EventBus.i.PlayerDeath += OnPlayerDeath;
        
        OnGameStarted?.Invoke();
    }
    
    public void FindReferences()
    {
        worldGenerator = GameObject.Find("WorldGen").GetComponent<WorldGenerator>();
        
        objectGenerator = GameObject.Find("WorldGen").GetComponent<ObjectGenerator>();
        
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();

        mainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        worldCanvas = GameObject.Find("WorldCanvas").GetComponent<Canvas>();
        
        squadCanvas = GameObject.Find("SquadCanvas").GetComponent<Canvas>();
        playerSquadPanel = GameObject.Find("SquadPanel").GetComponent<SquadPanel>();
        squadAssemblePanel = GameObject.Find("SquadAssemblePanel").GetComponent<SquadAssemblePanel>();
        
        buildingPanelManager = GameObject.Find("BuildingCanvas").GetComponent<BuildingPanelManager>();
        
        mouseFollower = GameObject.Find("MouseFollower").GetComponent<MouseFollower>();
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
    
    private void OnPlayerDeath()
    {
        if(ColoniesManager.i.playerColonies.Count <= 0){
            Helper.RestartCurrentScene();
        }
        else{
            StartCoroutine(RespawnPlayer(ColoniesManager.i.playerColonies[0].core.transform.position));
        }
    }
    
    public IEnumerator RespawnPlayer(Vector3 pos)
    {
        yield return new WaitForSeconds(2f);
        
        p.TakeDamage(-p.maxHealth);
        p.transform.position = pos;
        p.gameObject.SetActive(true);
        
        EventBus.i.PlayerRespawn?.Invoke();
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
