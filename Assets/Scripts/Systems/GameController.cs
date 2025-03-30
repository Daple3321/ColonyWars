using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController i { get; private set; }

    public static Player p;
    public static WorldGenerator worldGenerator;

    public InventoryUI playerInventoryUI; 
    public Transform pSpawnPoint;
    public Terrain currentTerrain;
    public GameSettings defaultGameSettings;

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

        ResetStaticVars();
    }

    public void ResetStaticVars()
    {
        p = null;
        worldGenerator = null;
    }

    // С параметрами старта (генерация мира, настройки, персонаж)
    private void StartGame(GameSettings gameSettings = null)
    {
        GameAssets.Init(); // это должно быть при запуске игры (в главном меню)
        if (gameSettings == null)
        {
            gameSettings = defaultGameSettings;
        }

        worldGenerator = GameObject.Find("WorldGen").GetComponent<WorldGenerator>();
        worldGenerator.Init(gameSettings.worldGenSettings);
        currentTerrain = worldGenerator.terrain;
        
        playerInventoryUI = GameObject.Find("PlayerInventory").GetComponent<InventoryUI>();

        GameObject playerObj = Instantiate(GameAssets.playerPrefab, pSpawnPoint.position, Quaternion.identity);
        p = playerObj.GetComponent<Player>();
        p.InitPlayer();
        
        // World gen
        // Reference assigning
    }

    void Start()
    {
        StartGame();
    }

}
