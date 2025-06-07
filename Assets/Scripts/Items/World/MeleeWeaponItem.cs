using UnityEngine;

public class MeleeWeaponItem : WeaponWorldItem
{
    [Space(7), Header("Melee weapon Settings")]
    public float attackDuration;
    public float attackDistance;
    public MeleeAttackType meleeAttackType;
    public float knockBackForce;
    
    
    public LayerMask hitLayers;
    private Transform shootPoint;
    private Player p;
    
    public override void Initialize(ItemData data, Item origin, int quantity=1)
    {
        base.Initialize(data, origin, quantity);

        if (data is MeleeWeaponData weaponData)
        {
            attackDuration = weaponData.attackDuration;
            attackDistance = weaponData.attackDistance;
            meleeAttackType = weaponData.meleeAttackType;
            knockBackForce = weaponData.knockBackForce;
        }
        
        p = GameController.p;
        shootPoint = GameController.p.shootPoint;
        cm = Camera.main;
    }
    
    public override void Attack(float concentraion)
    {
        base.Attack(concentraion);
        
        switch (meleeAttackType)
        {
            case MeleeAttackType.RAYCAST:
                RaycastAttack(concentraion);
                break;
            case MeleeAttackType.SPHERE:
                
                break;
            case MeleeAttackType.BOX:
                BoxCastAttack(concentraion);
                break;
        }
    }
    
    protected virtual void RaycastAttack(float concentraion)
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
                damageable.TakeDamage(originWeapon.damage.Value, GameController.p.gameObject, -hit.normal*knockBackForce);
            }
            
            //GameObject lineObj = Instantiate(hitScanLine, shootPoint.position, Quaternion.identity);
            //lineObj.GetComponent<HitscanLine>().Init(shootPoint.position, hit.point);
            Helper.SpawnHitEffect(hit.point, hit.normal, hit.collider.gameObject.layer);
        }
        else{
            //GameObject lineObj = Instantiate(hitScanLine, shootPoint.position, Quaternion.identity);
            //lineObj.GetComponent<HitscanLine>().Init(shootPoint.position, shootRay.GetPoint(6f));
            //Helper.SpawnHitEffect(mouseHit.point, mouseHit.normal, mouseHit.collider.gameObject.layer);
        }
        
        Debug.DrawRay(shootPoint.position, shootRay.direction, Color.cyan, 3);
        //PlayShootEffect();
    }
    
    protected virtual void BoxCastAttack(float concentraion)
    {
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = cm.ScreenPointToRay(mousePos);
        Ray shootRay = new Ray(shootPoint.position, mouseRay.direction);
        
        Vector3 boxPos = shootPoint.position + (p.shootPoint.forward * attackDistance);
        Vector3 boxSize = new Vector3(attackDistance, attackDistance, attackDistance);
        Collider[] hits = Physics.OverlapBox(boxPos, boxSize, p.shootPoint.rotation, hitLayers);
        
        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // go.transform.position = boxPos;
        // go.transform.localScale = boxSize;
        // go.transform.rotation = p.shootPoint.rotation;
        // Destroy(go.GetComponent<Collider>());
        // Destroy(go, 2);
        if (hits.Length > 0)
        {
            IDamageable damageable;
            foreach(Collider hit in hits)
            {
                if (hit.gameObject.TryGetComponent<IDamageable>(out damageable))
                {
                    damageable.TakeDamage(originWeapon.damage.Value, p.gameObject, -hit.transform.forward*knockBackForce);
                }
                
                Helper.SpawnHitEffect(hit.transform.position, -hit.transform.forward, hit.gameObject.layer);
            }
            
            //GameObject lineObj = Instantiate(hitScanLine, shootPoint.position, Quaternion.identity);
            //lineObj.GetComponent<HitscanLine>().Init(shootPoint.position, hit.point);
        }
        else{
            //GameObject lineObj = Instantiate(hitScanLine, shootPoint.position, Quaternion.identity);
            //lineObj.GetComponent<HitscanLine>().Init(shootPoint.position, shootRay.GetPoint(6f));
            //Helper.SpawnHitEffect(mouseHit.point, mouseHit.normal, mouseHit.collider.gameObject.layer);
        }
        
        Debug.DrawRay(shootPoint.position, shootRay.direction, Color.cyan, 3);
        //PlayShootEffect();
    }
}
