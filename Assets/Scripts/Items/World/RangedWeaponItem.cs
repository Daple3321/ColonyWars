using UnityEngine;

public class RangedWeaponItem : WeaponWorldItem
{
    public float shootDistance; // МОЖНО И НЕ ПЕРЕДАВАТЬ??
    public float shootInterval; // ДЕЛАТЬ ВСЮ ЛОГИКУ КД И ВСЕГО В itemOriginе?

    public Transform shootPoint;
    public Transform cartridgePos;

    public override void Initialize(ItemData data, Item origin)
    {
        base.Initialize(data, origin);

        if (data is RangedWeaponData weaponData)
        {
            shootDistance = weaponData.shootDistance;
            shootInterval = weaponData.shootInterval;
        }
    }
    
    public override void AttackEffects()
    {
        
    }
}
