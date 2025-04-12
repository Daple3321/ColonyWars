using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController i { get; private set; }

    public static Player p;
    public static WorldGenerator worldGenerator;
    public static TimeManager timeManager;

    //public InventoryUI inventoryUI;

    public Canvas mainCanvas;
    public MouseFollower mouseFollower;
    public Transform pSpawnPoint;
    public static Terrain currentTerrain;
    public GameSettings defaultGameSettings;
    
    private EventBus eventBus;
    
    void Awake()
    {
        if (i != null)
        {
            Destroy(this);
        }
        else
        {
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
        timeManager = null;
    }

    // С параметрами старта (генерация мира, настройки, персонаж)
    private void StartGame(GameSettings gameSettings = null)
    {
        ResetStaticVars();
        
        GameAssets.Init(); // это должно быть при запуске игры (в главном меню)
        if (gameSettings == null)
        {
            gameSettings = defaultGameSettings;
        }

        worldGenerator = GameObject.Find("WorldGen").GetComponent<WorldGenerator>();
        worldGenerator.Init(gameSettings.worldGenSettings);
        currentTerrain = worldGenerator.terrain;
        
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        timeManager.Init();

        //inventoryUI = GameObject.Find("PlayerInventory").GetComponent<InventoryUI>();
        mainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        mouseFollower = GameObject.Find("MouseFollower").GetComponent<MouseFollower>();

        GameObject playerObj = Instantiate(GameAssets.playerPrefab, pSpawnPoint.position, Quaternion.identity);
        p = playerObj.GetComponent<Player>();
        p.InitPlayer();
        
        
        EventBus.i.OnGameStarted?.Invoke();
        // World gen
        // Reference assigning
    }

    IEnumerator Start()
    {
        //yield return new WaitForSeconds(1);
        StartGame();
        yield return null;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            Helper.RestartCurrentScene();
        }
    }


    public static Vector3 GetPointOnTerrain(Vector3 point)
    {
        return new Vector3(point.x, currentTerrain.SampleHeight(point), point.z);
    }
}
