using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public enum DetectionShape{
        Sphere,
        Custom,
    }
    public DetectionShape detectionShape = DetectionShape.Sphere;
    public Collider detector;
    
    private LayerMask playerMask;
    private LayerMask enemyMask;
    private int playerUnitLayer;
    private int enemyUnitLayer;
    private Affiliation whoIsEnemy;
    private Defense owner;
    public void Init(Defense owner, Affiliation whoIsEnemy, float detectionSize)
    {
        playerMask = LayerMask.GetMask("PlayerBuilding", "Player", "PlayerUnit");
        enemyMask = LayerMask.GetMask("EnemyBuilding", "EnemyUnit");
        playerUnitLayer = LayerMask.NameToLayer("PlayerUnit");
        enemyUnitLayer = LayerMask.NameToLayer("EnemyUnit");
        
        this.owner = owner;
        this.whoIsEnemy = whoIsEnemy;
        
        switch(detectionShape)
        {
            case DetectionShape.Sphere:
                if(detector is SphereCollider sphereCollider){
                    sphereCollider.radius = detectionSize;
                }
            break;
            
            case DetectionShape.Custom:
                //detector.transform.localScale = new Vector3(1, 1, detectionSize);
            break;
        }
    }


    private void OnTriggerEnter(Collider col)
    {
        if(whoIsEnemy == Affiliation.Player && col.gameObject.layer == playerUnitLayer)
        {
            owner.AddTarget(col.GetComponent<Unit>());
            //Debug.Log("Player entered.", gameObject);
        }
        else if(whoIsEnemy == Affiliation.Enemy && col.gameObject.layer == enemyUnitLayer)
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
        if(whoIsEnemy == Affiliation.Player && col.gameObject.layer == playerUnitLayer)
        {
            owner.RemoveTarget(col.GetComponent<Unit>());
            //Debug.Log("Player exited.", gameObject);
        }
        else if(whoIsEnemy == Affiliation.Enemy && col.gameObject.layer == enemyUnitLayer)
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
