using System.Collections.Generic;
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
            playerColonies.Remove(pColony);
        }
        else if (colony is EnemyColony eColony){
            enemyColonies.Remove(eColony);
        }
    }
    
    public void SpawnEnemyColonies(GameSettings gameSettings)
    {
        
    }
}
