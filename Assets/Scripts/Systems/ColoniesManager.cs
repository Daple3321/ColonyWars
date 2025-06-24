using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MackySoft.Choice;

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
        gridManager = new GridManager(grid, borderPool, 9);
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
    
    // public void SpawnEnemyColonies(ColoniesSpawnSettings spawnSettings)
    // {
    //     for(int i = 0; i < spawnSettings.colonies; i++)
    //     {
    //         ColonyCenterData randColony = spawnSettings.coloniesPool[Random.Range(0, spawnSettings.coloniesPool.Length)];
    //         Building b = null;
    //         int maxIterations = 10;
    //         while(b == null && maxIterations > 0)
    //         {
    //             b = GameController.objectGenerator.CreateBuilding_Interval(randColony, spawnSettings.minColonyDistance, "EnemyColony", "PlayerColony");
    //             if(b != null){
    //                 b.GetComponent<EnemyColony>().Init(randColony);
    //             }
                
    //             maxIterations--;
    //         }
    //     }
    // }
    
    public void SpawnEnemyColonies(ColoniesSpawnSettings spawnSettings)
    {
        var selector = spawnSettings.colonyPool.ToWeightedSelector(x => x.Value);
        
        for(int i = 0; i < spawnSettings.colonies; i++)
        {
            //ColonyCenterData randColony = spawnSettings.coloniesPool[Random.Range(0, spawnSettings.coloniesPool.Length)];
            ColonyCenterData randColony = selector.SelectItemWithUnityRandom().Key;
            Building b = null;
            int maxIterations = 10;
            while(b == null && maxIterations > 0)
            {
                //Vector3Int randCell = gridManager.RandomCellIndex();
                Vector3 cellCenter = grid.GetCellCenterWorld(spawnSettings.spawnCell);
                Vector3 spawnPos = GameController.TerrainPoint(cellCenter);
                if(!gridManager.CheckCellForBuildings(spawnSettings.spawnCell.x, spawnSettings.spawnCell.z, Affiliation.Enemy))
                {
                    //Vector3 spawnPos = gridManager.RandomPointInCell(randCell.x, randCell.z);
                    
                    Quaternion rot = Quaternion.identity;
                    rot *= Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);
                    GameObject go = Instantiate(randColony.prefab, spawnPos, rot);
                    b = go.GetComponent<Building>();
                    b.Init(randColony);
                    
                    b.CaptureCellInstant(spawnSettings.spawnCell.x, spawnSettings.spawnCell.z);
                    //gridManager.cells[randCell.x, randCell.z].Capture(Affiliation.Enemy);
                }

                // if(b != null){
                //     b.GetComponent<EnemyColony>().Init(randColony);
                // }
                
                maxIterations--;
            }
        }
        
        //gridManager.ClearBorders();
        //gridManager.UpdateBorders();
    }
    
    public void SpawnEnemyCamps(ColoniesSpawnSettings spawnSettings)
    {
        var selector = spawnSettings.campPool.ToWeightedSelector(x => x.Value);
        
        int campsAmount = Random.Range(spawnSettings.campsRange.x, spawnSettings.campsRange.y);
        for(int i = 0; i < campsAmount; i++)
        {
            ColonyCenterData randColony = selector.SelectItemWithUnityRandom().Key;
            Building b = null;
            int maxIterations = 10;
            while(b == null && maxIterations > 0)
            {
                Vector3Int randCell = gridManager.RandomCellIndex();
                if(!gridManager.CheckCellForBuildings(randCell.x, randCell.z, Affiliation.Enemy))
                {
                    Vector3 spawnPos = gridManager.RandomPointInCell(randCell.x, randCell.z);
                    
                    Quaternion rot = Quaternion.identity;
                    rot *= Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);
                    GameObject go = Instantiate(randColony.prefab, spawnPos, rot);
                    b = go.GetComponent<Building>();
                    b.Init(randColony);
                    
                    //b.CaptureCellInstant(randCell.x, randCell.z);
                }
                
                maxIterations--;
            }
        }
    }
}
