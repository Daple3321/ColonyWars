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

    public List<Colony> playerColonies;
    
    public List<Colony> enemyColonies;
    
    public void Init()
    {
        playerColonies = new List<Colony>();
        enemyColonies = new List<Colony>();
        
        EventBus.i.OnColonyCreated += OnColonyCreated;
        EventBus.i.OnColonyDestroyed += OnColonyDestroyed;
    }
    
    public void OnColonyCreated(Colony newColony)   
    {
        if (newColony.affiliation == Affiliation.Player){
            playerColonies.Add(newColony);
        }
        else if (newColony.affiliation == Affiliation.Enemy){
            enemyColonies.Add(newColony);
        }
    }
    public void OnColonyDestroyed(Colony colony)   
    {
        if (colony.affiliation == Affiliation.Player){
            playerColonies.Remove(colony);
        }
        else if (colony.affiliation == Affiliation.Enemy){
            enemyColonies.Remove(colony);
        }
    }
}
