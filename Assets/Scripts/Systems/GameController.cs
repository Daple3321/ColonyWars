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
        ColoniesManager.i.SpawnEnemyColonies(gameSettings.coloniesSpawnSettings);
        
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
            StartCoroutine(RespawnPlayer(ColoniesManager.i.playerColonies[0].transform.position));
        }
    }
    
    public IEnumerator RespawnPlayer(Vector3 pos)
    {
        yield return new WaitForSeconds(2f);
        
        p.health = p.maxHealth;
        p.TakeDamage(p.maxHealth/2);
        PlayerAiming.isAiming = false;
        p.transform.position = pos;
        p.gameObject.SetActive(true);
        
        EventBus.i.PlayerRespawn?.Invoke();
    }
    

    public static Vector3 TerrainPoint(Vector3 point)
    {
        return new Vector3(point.x, currentTerrain.SampleHeight(point), point.z);
    }
    public static float GetTerrainAngle(Vector3 point)
    {
        float angle = 0;
        
        Vector3 rayOrigin = new Vector3(point.x, 50, point.z);
        Ray ray = new Ray(rayOrigin, Vector3.down);
        RaycastHit[] hit = new RaycastHit[1];
        if(Physics.RaycastNonAlloc(ray, hit, Mathf.Infinity, LayerMask.GetMask("Ground")) > 0)
        {
            angle = Vector3.Angle(Vector3.up, hit[0].normal);
        }
        
        return angle;
    }
    public static Vector2 RandomPointInCircle(Vector2 origin, float minRadius, float maxRadius)
    {
        Vector2 randomDirection = (Random.insideUnitCircle * origin).normalized;
        float randomDistance = Random.Range(minRadius, maxRadius);
        Vector2 point = origin + randomDirection * randomDistance;

        return point;
    }
    public static Vector3 RandomPointInCircleTerrain(Vector2 origin, float minRadius, float maxRadius) // выходит за террейн если origin близко к краю!
    {
        Vector2 randomDirection = (Random.insideUnitCircle.normalized * origin).normalized;
        //Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minRadius, maxRadius);
        Vector2 point = origin + randomDirection * randomDistance;
        
        Vector3 terrainPoint = new Vector3(point.x, currentTerrain.SampleHeight(new Vector3(point.x, 0, point.y)), point.y);

        return terrainPoint;
    }
    
    /*public static Vector3 RandomPointInAnnulusTerrain(Vector2 origin, float minRadius, float maxRadius)
    {
        // 1. Получаем случайное направление
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        
        // (Опционально) Обработка крайне редкого случая, когда Random.insideUnitCircle вернет (0,0)
        if (randomDirection == Vector2.zero)
        {
            randomDirection = Vector2.right; // Задаем направление по умолчанию
        }

        // 2. Получаем случайное расстояние в заданном диапазоне
        float randomDistance = Random.Range(minRadius, maxRadius);

        // 3. Вычисляем 2D точку на плоскости XZ
        Vector2 pointXZ = origin + randomDirection * randomDistance;
        
        // 4. Получаем высоту террейна в этой точке
        // Убедитесь, что currentTerrain назначен и это нужный террейн!
        if (currentTerrain == null)
        {
            Debug.LogError("currentTerrain не назначен!");
            // Возвращаем точку на заданной высоте или обрабатываем ошибку иначе
            return new Vector3(pointXZ.x, 0, pointXZ.y); 
        }

        // Передаем Vector3 в SampleHeight, где Y обычно игнорируется, но для ясности можно задать 0
        float terrainHeight = currentTerrain.SampleHeight(new Vector3(pointXZ.x, 0, pointXZ.y));
        
        // 5. Создаем итоговую 3D точку с корректной высотой
        Vector3 terrainPoint = new Vector3(pointXZ.x, terrainHeight, pointXZ.y);

        return terrainPoint;
    }*/
}
