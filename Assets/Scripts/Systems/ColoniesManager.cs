using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColoniesManager : MonoBehaviour
{
    public static ColoniesManager i;

    void Awake()
    {
        if (i != null){
            Destroy(this);
        }
        else{
            i = this;
        }
    }

    public List<PlayerColony> playerColonies;
    
    public List<EnemyColony> enemyColonies;

    public BorderPool borderPool;
    public GridManager gridManager;
    public Grid grid;

    public void Init()
    {
        playerColonies = new List<PlayerColony>();
        enemyColonies = new List<EnemyColony>();

        EventBus.i.OnColonyCreated += OnColonyCreated;
        EventBus.i.OnColonyDestroyed += OnColonyDestroyed;

        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // go.name = "0_0 center";
        // go.transform.position = grid.GetCellCenterWorld(new Vector3Int(0, 0, 0));
        // Bounds bd = grid.GetBoundsLocal(new Vector3Int(0, 0, 0), new Vector3(3, 1, 3));
        // Debug.Log(bd);
        // go.transform.localScale = bd.size;
        gridManager = new GridManager(grid, borderPool, 10);
    }
    
    public void OnColonyCreated(Colony newColony)   
    {
        if (newColony is PlayerColony pColony){
            playerColonies.Add(pColony);
        }
        else if (newColony is EnemyColony eColony){
            enemyColonies.Add(eColony);
        }
    }
    public void OnColonyDestroyed(Colony colony)   
    {
        if (colony is PlayerColony pColony){
            if(playerColonies.Count == 1){
                // Lose
            }
            
            playerColonies.Remove(pColony);
        }
        else if (colony is EnemyColony eColony){
            enemyColonies.Remove(eColony);
        }
    }
    
    public void SpawnEnemyColonies(ColoniesSpawnSettings spawnSettings)
    {
        for(int i = 0; i < spawnSettings.colonies; i++)
        {
            ColonyCenterData randColony = spawnSettings.coloniesPool[Random.Range(0, spawnSettings.coloniesPool.Length)];
            Building b = null;
            int maxIterations = 10;
            while(b == null && maxIterations > 0)
            {
                b = GameController.objectGenerator.CreateBuilding_Interval(randColony, spawnSettings.minColonyDistance, "EnemyColony", "PlayerColony");
                if(b != null){
                    b.GetComponent<EnemyColony>().Init(randColony);
                }
                
                maxIterations--;
            }
        }
    }
}
