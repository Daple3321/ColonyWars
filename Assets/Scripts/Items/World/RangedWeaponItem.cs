using UnityEngine;

public class RangedWeaponItem : WeaponWorldItem
{
    public float shootDistance; // МОЖНО И НЕ ПЕРЕДАВАТЬ??
    public ShootStyle shootStyle;
    public AnimationCurve concentrationScatter;

    public float projectileSpeed;
    public int penetrationAmount;
    public int projectilesPerShot;
    public float projectileLifetime;
    public float knockBackForce;
    public GameObject projectilePrefab;
    public LayerMask hitLayers;
    public Transform shootPoint;
    public Transform cartridgePos;

    public GameObject hitScanLine;
    public ParticleSystem shootEffect;
    public GameObject hitEffect;
    private Camera cm;

    public override void Initialize(ItemData data, Item origin, int quantity=1)
    {
        base.Initialize(data, origin, quantity);

        if (data is RangedWeaponData weaponData)
        {
            shootDistance = weaponData.shootDistance;
            projectileSpeed = weaponData.projectileSpeed;
            projectileLifetime = weaponData.projectileLifetime;
            penetrationAmount = weaponData.penetrationAmount;
            projectilesPerShot = weaponData.projectilesPerShot;
            knockBackForce = weaponData.knockBackForce;
            shootStyle = weaponData.shootStyle;
            concentrationScatter = weaponData.concentrationScatter;
        }
        
        cm = Camera.main;
    }

    public override void Attack(float concentraion) // ВЫСТРЕЛЫ ПРОСТО НЕ СПАВНЯТСЯ ЕСЛИ НИЧЕГО НЕ ХИТАНУЛИ.
    {
        base.Attack(concentraion);
        
        switch (shootStyle)
        {
            case ShootStyle.HITSCAN:
                HitscanShot(concentraion);
                break;
            case ShootStyle.PROJECTILE:
                ProjectileShot(concentraion);
                break;
            case ShootStyle.AREA_HITSCAN:
                
                //PlayShootEffect();
                break;
        }
    }

    protected virtual void PlayShootEffect()
    {
        shootEffect.Play();
    }

    protected virtual void ProjectileShot(float concentraion)
    {
        // Vector3 mousePos = Input.mousePosition;
        // Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
        // RaycastHit mouseHit;
        // if (Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity, hitLayers))
        // {
        //     Vector3 shootDir = mouseHit.point - shootPoint.position;
        //     float scatterAmount = concentrationScatter.Evaluate(concentraion);
        //     Ray shootRay = new Ray(shootPoint.position, Helper.GetRandPointOnUnitSphereCap(shootDir, scatterAmount));
        //     Projectile projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity).GetComponent<Projectile>();
        //     //projectile.transform.rotation.SetLookRotation(shootRay.direction, projectile.transform.up);
        //     projectile.transform.up = shootRay.direction;
        //     projectile.Init(damage, projectileSpeed, Affiliation.Player, penetrationAmount, projectileLifetime);
            
        //     Debug.DrawLine(shootPoint.position, mouseHit.point, Color.green, 2);
        //     Debug.DrawRay(shootPoint.position, shootRay.direction * 4, Color.cyan, 3);
        // }
        
        /*RaycastHit mouseHit;
        if(PlayerAiming.Raycast(hitLayers, out mouseHit))
        {
            Ray mouseRay = cm.ScreenPointToRay(mousePos);
            RaycastHit mouseHit;
            
            Vector3 shootDir = mouseHit.point - shootPoint.position;
            
            for(int i = 0; i < projectilesPerShot; i++)
            {
                float scatterAmount = concentrationScatter.Evaluate(concentraion);
                Ray shootRay = new Ray(shootPoint.position, Helper.GetRandPointOnUnitSphereCap(shootDir, scatterAmount));
                Projectile projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity).GetComponent<Projectile>();
                //projectile.transform.rotation.SetLookRotation(shootRay.direction, projectile.transform.up);
                projectile.transform.up = shootRay.direction;
                projectile.Init(damage, projectileSpeed, Affiliation.Player, penetrationAmount, projectileLifetime);
                
                Debug.DrawLine(shootPoint.position, mouseHit.point, Color.green, 2);
                Debug.DrawRay(shootPoint.position, shootRay.direction * 4, Color.cyan, 3);

            }
            
            PlayShootEffect();
        }*/
        
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = cm.ScreenPointToRay(mousePos);
        //RaycastHit mouseHit;
        //Vector3 shootDir = mouseHit.point - shootPoint.position;
        
        for(int i = 0; i < projectilesPerShot; i++)
        {
            float scatterAmount = concentrationScatter.Evaluate(concentraion);
            Ray shootRay = new Ray(shootPoint.position, Helper.GetRandPointOnUnitSphereCap(mouseRay.direction, scatterAmount));
            
            Projectile projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity).GetComponent<Projectile>();
            //projectile.transform.rotation.SetLookRotation(shootRay.direction, projectile.transform.up);
            projectile.transform.up = shootRay.direction;
            projectile.Init(damage, projectileSpeed, Affiliation.Player, penetrationAmount, projectileLifetime);
            
            //Debug.DrawLine(shootPoint.position, mouseHit.point, Color.green, 2);
            Debug.DrawRay(shootPoint.position, mouseRay.direction * 4, Color.cyan, 3);
        }
        
        PlayShootEffect();
    }
    
    protected virtual void HitscanShot(float concentraion)
    {
        // Vector3 mousePos = Input.mousePosition;
        // Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
        // RaycastHit mouseHit;
        // if (Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity, hitLayers))
        // {
        //     Vector3 shootDir = mouseHit.point - shootPoint.position;
        //     float scatterAmount = concentrationScatter.Evaluate(concentraion);
        //     Ray shootRay = new Ray(shootPoint.position, Helper.GetRandPointOnUnitSphereCap(shootDir, scatterAmount));
        //     RaycastHit hit;
        //     if (Physics.Raycast(shootRay, out hit, shootDistance, hitLayers)) // layermask
        //     {
        //         Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.zero, hit.normal));
        //         GameObject lineObj = Instantiate(hitScanLine, shootPoint.position, Quaternion.FromToRotation(Vector3.zero, hit.point - shootPoint.position));
        //         //lineObj.GetComponent<LineRenderer>().SetPosition(1, hit.point);
                
        //         Debug.Log($"Hit {hit.collider.name}");
        //         Debug.DrawLine(shootPoint.position, hit.point, Color.green, 2);
        //     }
        //     Debug.DrawRay(shootPoint.position, shootRay.direction, Color.cyan, 3);
        // }
        
        /*RaycastHit mouseHit;
        if(PlayerAiming.Raycast(hitLayers, out mouseHit))
        {
            Vector3 shootDir = mouseHit.point - shootPoint.position;
            
            for(int i = 0; i < projectilesPerShot; i++)
            {
                float scatterAmount = concentrationScatter.Evaluate(concentraion);
                Ray shootRay = new Ray(shootPoint.position, Helper.GetRandPointOnUnitSphereCap(shootDir, scatterAmount));
                RaycastHit hit;
                if (Physics.Raycast(shootRay, out hit, shootDistance, hitLayers))
                {
                    IDamageable damageable;
                    if (hit.collider.gameObject.TryGetComponent<IDamageable>(out damageable))
                    {
                        damageable.TakeDamage(damage, -hit.normal*knockBackForce);
                    }
                    
                    GameObject lineObj = Instantiate(hitScanLine, shootPoint.position, Quaternion.identity);
                    lineObj.GetComponent<HitscanLine>().Init(shootPoint.position, hit.point);
                    Helper.SpawnHitEffect(hit.point, hit.normal, hit.collider.gameObject.layer);
                }
                else{
                    GameObject lineObj = Instantiate(hitScanLine, shootPoint.position, Quaternion.identity);
                    lineObj.GetComponent<HitscanLine>().Init(shootPoint.position, hit.point);
                    //Helper.SpawnHitEffect(mouseHit.point, mouseHit.normal, mouseHit.collider.gameObject.layer);
                }
                
                
                Debug.DrawRay(shootPoint.position, shootRay.direction, Color.cyan, 3);
            }
            
            PlayShootEffect();
        }*/
        
        
        //Vector3 shootDir = mouseHit.point - shootPoint.position;
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = cm.ScreenPointToRay(mousePos);
        for(int i = 0; i < projectilesPerShot; i++)
        {
            float scatterAmount = concentrationScatter.Evaluate(concentraion);
            Ray shootRay = new Ray(shootPoint.position, Helper.GetRandPointOnUnitSphereCap(mouseRay.direction, scatterAmount));
            RaycastHit hit;
            if (Physics.Raycast(shootRay, out hit, shootDistance, hitLayers))
            {
                IDamageable damageable;
                if (hit.collider.gameObject.TryGetComponent<IDamageable>(out damageable))
                {
                    damageable.TakeDamage(damage, -hit.normal*knockBackForce);
                }
                
                GameObject lineObj = Instantiate(hitScanLine, shootPoint.position, Quaternion.identity);
                lineObj.GetComponent<HitscanLine>().Init(shootPoint.position, hit.point);
                Helper.SpawnHitEffect(hit.point, hit.normal, hit.collider.gameObject.layer);
            }
            else{
                GameObject lineObj = Instantiate(hitScanLine, shootPoint.position, Quaternion.identity);
                lineObj.GetComponent<HitscanLine>().Init(shootPoint.position, shootRay.GetPoint(6f));
                //Helper.SpawnHitEffect(mouseHit.point, mouseHit.normal, mouseHit.collider.gameObject.layer);
            }
            
            Debug.DrawRay(shootPoint.position, shootRay.direction, Color.cyan, 3);
        }
        PlayShootEffect();
        
    }
}
