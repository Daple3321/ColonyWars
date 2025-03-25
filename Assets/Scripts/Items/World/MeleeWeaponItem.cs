using UnityEngine;

public class MeleeWeaponItem : WeaponWorldItem
{
    public float attackDuration;
    public float attackCharge;
    public float attackInterval;
    
    public override void Initialize(ItemData data, Item origin)
    {
        base.Initialize(data, origin);

        if (data is MeleeWeaponData weaponData)
        {
            attackDuration = weaponData.attackDuration;
            attackCharge = weaponData.attackCharge;
            attackInterval = weaponData.attackInterval;
        }
    }
}
