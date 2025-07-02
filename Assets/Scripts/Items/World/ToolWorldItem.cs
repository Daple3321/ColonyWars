using UnityEngine;

public class ToolWorldItem : MeleeWeaponItem
{
    public GameObject gatherEffect;
    
    
    private ToolData toolData;
    public override void Initialize(ItemData data, Item origin, int quantity=1)
    {
        base.Initialize(data, origin, quantity);

        if (data is ToolData td)
        {
            toolData = td;
            attackDistance = td.attackDistance;
            meleeAttackType = td.meleeAttackType;
            knockBackForce = td.knockBackForce;
            attackBoxExtents = td.attackBoxExtents;
        }
        
        p = GameController.p;
        shootPoint = GameController.p.shootPoint;
        cm = Camera.main;
    }
    
    public override void Attack(float concentraion)
    {
        if(impulseSource != null){
            impulseSource.GenerateImpulse();
        }
        
        switch (meleeAttackType)
        {
            case MeleeAttackType.RAYCAST:
                RaycastAttack(concentraion);
                break;
            case MeleeAttackType.SPHERE:
                Debug.LogWarning("Not implemented yet.", gameObject);
                break;
            case MeleeAttackType.BOX:
                BoxCastAttack(concentraion);
                break;
        }
    }
    
    protected override async void RaycastAttack(float concentraion)
    {
        //Vector3 shootDir = mouseHit.point - shootPoint.position;
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = cm.ScreenPointToRay(mousePos);
        
        Ray shootRay = new Ray(shootPoint.position, mouseRay.direction);
        RaycastHit hit;
        if (Physics.Raycast(shootRay, out hit, attackDistance, hitLayers))
        {
            IDamageable damageable;
            if (hit.collider.gameObject.TryGetComponent<IDamageable>(out damageable))
            {
                var result = await damageable.TakeDamage(originWeapon.damage.Value, GameController.p.gameObject, -hit.normal*knockBackForce);
                //Debug.Log($"Hit result: {result}");
            }
            
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Resources") 
                && ResourceManager.i.CanGather(hit.collider.gameObject, toolData))
            {
                ResourceManager.i.PlayerToolHarvest(hit.collider.gameObject, Random.Range(toolData.yieldRange.x, toolData.yieldRange.y));
                
                Helper.SpawnEffect(gatherEffect, hit.point, hit.normal);
            }
            
            Helper.SpawnHitEffect(hit.point, hit.normal, hit.collider.gameObject.layer);
        }
        
        Debug.DrawRay(shootPoint.position, shootRay.direction, Color.cyan, 3);
    }
    
    protected override async void BoxCastAttack(float concentraion)
    {
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = cm.ScreenPointToRay(mousePos);
        Ray shootRay = new Ray(shootPoint.position, mouseRay.direction);
        
        Vector3 boxPos = shootPoint.position + (p.shootPoint.forward * (attackBoxExtents.z/2));
        Vector3 boxSize = attackBoxExtents;
        Collider[] hits = Physics.OverlapBox(boxPos, boxSize, p.shootPoint.rotation, hitLayers);
        
        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // go.transform.position = boxPos;
        // go.transform.localScale = boxSize;
        // go.transform.rotation = p.shootPoint.rotation;
        // Destroy(go.GetComponent<Collider>());
        // Destroy(go, 1.5f);
        if (hits.Length > 0)
        {
            IDamageable damageable;
            foreach(Collider hit in hits)
            {
                if (hit.gameObject.TryGetComponent<IDamageable>(out damageable))
                {
                    var result = await damageable.TakeDamage(originWeapon.damage.Value, p.gameObject, -hit.transform.forward*knockBackForce);
                }
                
                if(hit.gameObject.layer == LayerMask.NameToLayer("Resources") 
                && ResourceManager.i.CanGather(hit.gameObject, toolData))
                {
                    ResourceManager.i.PlayerToolHarvest(hit.gameObject, Random.Range(toolData.yieldRange.x, toolData.yieldRange.y));
                    
                    Helper.SpawnEffect(gatherEffect, hit.transform.position, -hit.transform.forward);
                }
                
                Helper.SpawnHitEffect(hit.transform.position, -hit.transform.forward, hit.gameObject.layer);
            }
        }
        
        Debug.DrawRay(shootPoint.position, shootRay.direction, Color.cyan, 3);
    }
}
