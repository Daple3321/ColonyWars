using UnityEditor;
using UnityEngine;

public class RangedWeaponItem : WeaponWorldItem
{
    public float shootDistance; // МОЖНО И НЕ ПЕРЕДАВАТЬ??
    public float shootInterval; // ДЕЛАТЬ ВСЮ ЛОГИКУ КД И ВСЕГО В itemOriginе?
    public AnimationCurve concentrationScatter;

    public LayerMask hitLayers;
    public Transform shootPoint;
    public Transform cartridgePos;

    public ParticleSystem shootEffect;

    public override void Initialize(ItemData data, Item origin)
    {
        base.Initialize(data, origin);

        if (data is RangedWeaponData weaponData)
        {
            shootDistance = weaponData.shootDistance;
            shootInterval = weaponData.shootInterval;
        }
    }

    public override void Attack(float concentraion) // ВЫСТРЕЛЫ ПРОСТО НЕ СПАВНЯТСЯ ЕСЛИ НИЧЕГО НЕ ХИТАНУЛИ.
    {
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit mouseHit;
        if (Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity, hitLayers))
        {
            Vector3 shootDir = mouseHit.point - shootPoint.position;
            float scatterAmount = concentrationScatter.Evaluate(concentraion);
            Ray shootRay = new Ray(shootPoint.position, Helper.GetRandPointOnUnitSphereCap(shootDir, scatterAmount));
            RaycastHit hit;
            if (Physics.Raycast(shootRay, out hit, Mathf.Infinity, hitLayers)) // layermask
            {
                Instantiate(GameAssets.itemPrefab, hit.point, Quaternion.FromToRotation(Vector3.zero, hit.normal));
                Debug.Log($"Hit {hit.collider.name}");
                Debug.DrawLine(shootPoint.position, hit.point, Color.green, 2);
            }
            Debug.DrawRay(shootPoint.position, shootRay.direction, Color.cyan, 3);
        }
    }
}
