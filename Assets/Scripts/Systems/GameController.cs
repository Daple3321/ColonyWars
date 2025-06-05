using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using static EntityStatType;

public class GameController : MonoBehaviour
{
    public static GameController i { get; private set; }

    public static Player p;
    public static WorldGenerator worldGenerator;
    public static ObjectGenerator objectGenerator;
    public static TimeManager timeManager;

    //public InventoryUI inventoryUI;
    
    public PointStats playerPoints;
    public PointStats enemyPoints;
    public int pointsToWin;
    public float pointsMultiplier;

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
    public static event Action<Affiliation> OnGameEnded; // arg - who won
    public bool gameEnded = false;
    private async void StartGame(GameSettings gameSettings = null)
    {
        Application.targetFrameRate = 120;
        
        gameEnded = false;
        ResetStaticVars();
        
        GameAssets.Init(); // это должно быть при запуске игры (в главном меню)
        if (gameSettings == null){
            gameSettings = defaultGameSettings;
        }
        pointsToWin = gameSettings.pointsToWin;
        pointsMultiplier = gameSettings.pointsMultiplier;
        
        playerPoints = new PointStats(pointsToWin);
        playerPoints.onPointsChanged += (x, y)=>{EventBus.i.OnPlayerPointsChanged?.Invoke(x, y);};
        playerPoints.onYieldChanged += x => {EventBus.i.OnPlayerYieldChanged?.Invoke(x);};
        
        enemyPoints = new PointStats(pointsToWin);
        enemyPoints.onPointsChanged += (x, y)=>{EventBus.i.OnEnemyPointsChanged?.Invoke(x, y);};
        enemyPoints.onYieldChanged += x => {EventBus.i.OnEnemyYieldChanged?.Invoke(x);};

        FindReferences();

        worldGenerator.Init(gameSettings.worldGenSettings);
        currentTerrain = worldGenerator.terrain;
        objectGenerator.Init(gameSettings.objectGenSettings);
        timeManager.Init();
        //await worldGenerator.GenerateTerrain();
        //await objectGenerator.GenerateObjects();
        //Pools.Init();

        //inventoryUI = GameObject.Find("PlayerInventory").GetComponent<InventoryUI>();
        
        ColoniesManager.i.Init();
        ColoniesManager.i.SpawnEnemyColonies(gameSettings.coloniesSpawnSettings);
        ColoniesManager.i.SpawnEnemyCamps(gameSettings.coloniesSpawnSettings);
        
        //pSpawnPoint.transform.position = RandomPointOnMap();
        GameObject playerObj = Instantiate(GameAssets.playerPrefab, pSpawnPoint.position, Quaternion.identity);
        p = playerObj.GetComponent<Player>();
        p.InitPlayer();
        
        worldCanvas.worldCamera = Camera.main; // after player
        
        WorldUI.i.Init();
        
        EventBus.i.PlayerDeath += OnPlayerDeath;
        EventBus.i.OnMinuteChange += IncrementPoints;
        
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
        HandleCheats();
    }
    
    public Vector3 RandomPointOnMap()
    {
        Vector3Int cell = ColoniesManager.i.gridManager.RandomCellIndex();
        return ColoniesManager.i.gridManager.RandomPointInCell(cell.x, cell.z);
    }
    
    public void AddPoints(float amount)
    {
        
    }
    private void IncrementPoints()
    {
        if(gameEnded)
            return;
        
        playerPoints.IncrementPoints();
        enemyPoints.IncrementPoints();
        
        CheckWin();
    }
    private void CheckWin()
    {
        if(playerPoints.points >= pointsToWin && enemyPoints.points < pointsToWin)
        {
            Debug.Log("Player WON!");
            OnGameEnded?.Invoke(Affiliation.Player);
            
            gameEnded = true;
        }
        else if(playerPoints.points < pointsToWin && enemyPoints.points >= pointsToWin)
        {
            Debug.Log("Enemy WON!");
            OnGameEnded?.Invoke(Affiliation.Enemy);
            
            gameEnded = true;
        }
        else if(playerPoints.points >= pointsToWin && enemyPoints.points >= pointsToWin)
        {
            Debug.Log("DRAW");
            OnGameEnded?.Invoke(Affiliation.None);
            
            gameEnded = true;
        }
    }
    public void UpdatePointStats(List<Cell> playerCells, List<Cell> enemyCells)
    {
        float playerYield = 0;
        foreach(Cell cell in playerCells)
        {
            playerYield += cell.yieldAmount;       
        }
        playerPoints.SetYield(playerYield);
        
        float enemyYield = 0;
        foreach(Cell cell in enemyCells)
        {
            enemyYield += cell.yieldAmount;       
        }
        enemyPoints.SetYield(enemyYield);
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
        
        p.health = p.stats[maxHealth].Value;
        PlayerAiming.isAiming = false;
        p.transform.position = pos;
        p.gameObject.SetActive(true);
        p.TakeDamage(p.stats[maxHealth].Value/2);
        
        EventBus.i.PlayerRespawn?.Invoke();
    }
    
    private void HandleCheats()
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
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            foreach(Collider c in ColoniesManager.i.gridManager.GetCellBuildings(0, 0))
            {
                Debug.Log(c.name);
            }
        }
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

[System.Serializable]
public class PointStats
{
    public int pointsToWin;
    public PointStats(int pointsToWin){
        this.pointsToWin = pointsToWin;
    }
    
    public float points = 0;
    public float yield;
    
    public float personalMultiplier = 1f;
    
    public event Action<float, float> onPointsChanged;
    public event Action<float> onYieldChanged;
    
    public void SetYield(float amount)
    {
        yield = amount;
        
        onYieldChanged?.Invoke(amount);
    }
    
    public void AddPoints(float amount)
    {
        points += amount;
        
        onPointsChanged?.Invoke(points, pointsToWin);
    }
    public void IncrementPoints()
    {
        points += yield * personalMultiplier;
        
        onPointsChanged?.Invoke(points, pointsToWin);
    }
}