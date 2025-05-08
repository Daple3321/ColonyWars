using UnityEngine;

public class MeleeWeaponItem : WeaponWorldItem
{
    public float attackDuration;
    public float attackInterval;
    
    public override void Initialize(ItemData data, Item origin, int quantity=1)
    {
        base.Initialize(data, origin, quantity);

        if (data is MeleeWeaponData weaponData)
        {
            attackDuration = weaponData.attackDuration;
            attackInterval = weaponData.attackInterval;
        }
    }
}
