using UnityEngine;

public class RangedWeaponItem : WeaponWorldItem
{
    public float shootDistance; // МОЖНО И НЕ ПЕРЕДАВАТЬ??
    public ShootStyle shootStyle;
    public AnimationCurve concentrationScatter;

    public float projectileSpeed;
    public int penetrationAmount;
    public float projectileLifetime;
    public GameObject projectilePrefab;
    public LayerMask hitLayers;
    public Transform shootPoint;
    public Transform cartridgePos;

    public GameObject hitScanLine;
    public ParticleSystem shootEffect;
    public GameObject hitEffect;

    public override void Initialize(ItemData data, Item origin, int quantity=1)
    {
        base.Initialize(data, origin, quantity);

        if (data is RangedWeaponData weaponData)
        {
            shootDistance = weaponData.shootDistance;
            projectileSpeed = weaponData.projectileSpeed;
            projectileLifetime = weaponData.projectileLifetime;
            penetrationAmount = weaponData.penetrationAmount;
            shootStyle = weaponData.shootStyle;
            concentrationScatter = weaponData.concentrationScatter;
        }
    }

    public override void Attack(float concentraion) // ВЫСТРЕЛЫ ПРОСТО НЕ СПАВНЯТСЯ ЕСЛИ НИЧЕГО НЕ ХИТАНУЛИ.
    {
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
        
        RaycastHit mouseHit;
        if(PlayerAiming.Raycast(hitLayers, out mouseHit))
        {
            Vector3 shootDir = mouseHit.point - shootPoint.position;
            float scatterAmount = concentrationScatter.Evaluate(concentraion);
            Ray shootRay = new Ray(shootPoint.position, Helper.GetRandPointOnUnitSphereCap(shootDir, scatterAmount));
            Projectile projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity).GetComponent<Projectile>();
            //projectile.transform.rotation.SetLookRotation(shootRay.direction, projectile.transform.up);
            projectile.transform.up = shootRay.direction;
            projectile.Init(damage, projectileSpeed, Affiliation.Player, penetrationAmount, projectileLifetime);
            
            PlayShootEffect();
            Debug.DrawLine(shootPoint.position, mouseHit.point, Color.green, 2);
            Debug.DrawRay(shootPoint.position, shootRay.direction * 4, Color.cyan, 3);
        }
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
        
        RaycastHit mouseHit;
        if(PlayerAiming.Raycast(hitLayers, out mouseHit))
        {
            Vector3 shootDir = mouseHit.point - shootPoint.position;
            float scatterAmount = concentrationScatter.Evaluate(concentraion);
            Ray shootRay = new Ray(shootPoint.position, Helper.GetRandPointOnUnitSphereCap(shootDir, scatterAmount));
            RaycastHit hit;
            if (Physics.Raycast(shootRay, out hit, shootDistance, hitLayers))
            {
                Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.zero, hit.normal));
                GameObject lineObj = Instantiate(hitScanLine, shootPoint.position, Quaternion.FromToRotation(Vector3.zero, hit.point - shootPoint.position));
                //lineObj.GetComponent<LineRenderer>().SetPosition(1, hit.point);
                
                Debug.Log($"Hit {hit.collider.name}");
                Debug.DrawLine(shootPoint.position, hit.point, Color.green, 2);
            }
            
            PlayShootEffect();
            Debug.DrawRay(shootPoint.position, shootRay.direction, Color.cyan, 3);
        }
    }
}
