using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public enum DetectionShape{
        Sphere,
        Custom,
        Box,
    }
    public DetectionShape detectionShape = DetectionShape.Sphere;
    
    private LayerMask playerMask;
    private LayerMask enemyMask;
    private Affiliation whoIsEnemy;
    private Defense owner;
    public void Init(Defense owner, Affiliation whoIsEnemy)
    {
        playerMask = LayerMask.GetMask("PlayerBuilding", "Player", "PlayerUnit");
        enemyMask = LayerMask.GetMask("EnemyBuilding", "EnemyUnit");
        
        this.owner = owner;
        this.whoIsEnemy = whoIsEnemy;
    }


    private void OnTriggerEnter(Collider col)
    {
        if(whoIsEnemy == Affiliation.Player && col.gameObject.layer == LayerMask.NameToLayer("PlayerUnit"))
        {
            owner.AddTarget(col.GetComponent<Unit>());
            //Debug.Log("Player entered.", gameObject);
        }
        else if(whoIsEnemy == Affiliation.Enemy && col.gameObject.layer == LayerMask.NameToLayer("EnemyUnit"))
        {
            owner.AddTarget(col.GetComponent<Unit>());
            //Debug.Log("Enemy entered.", gameObject);
        }
        
        if(col.gameObject.layer == 6 && whoIsEnemy == Affiliation.Player) // player
        {
            owner.playerTarget = GameController.p;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if(whoIsEnemy == Affiliation.Player && col.gameObject.layer == LayerMask.NameToLayer("PlayerUnit"))
        {
            owner.RemoveTarget(col.GetComponent<Unit>());
            //Debug.Log("Player exited.", gameObject);
        }
        else if(whoIsEnemy == Affiliation.Enemy && col.gameObject.layer == LayerMask.NameToLayer("EnemyUnit"))
        {
            owner.RemoveTarget(col.GetComponent<Unit>());
            //Debug.Log("Enemy exited.", gameObject);
        }
        
        if(col.gameObject.layer == 6) // player
        {
            owner.playerTarget = null;
        }
    }
}
