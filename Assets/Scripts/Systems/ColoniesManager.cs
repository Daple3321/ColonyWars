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
    
    public void Init()
    {
        playerColonies = new List<PlayerColony>();
        enemyColonies = new List<EnemyColony>();
        
        EventBus.i.OnColonyCreated += OnColonyCreated;
        EventBus.i.OnColonyDestroyed += OnColonyDestroyed;
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
            
            Building b = GameController.objectGenerator.CreateBuilding_Interval(randColony, spawnSettings.minColonyDistance, "EnemyColony", "PlayerColony");
            b.GetComponent<EnemyColony>().Init(randColony);
        }
    }
}
