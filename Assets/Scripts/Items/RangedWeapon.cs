using UnityEngine;

public class RangedWeapon : Weapon
{
    public override void Attack()
    {
        Debug.Log($"Ranged attack with: {itemName}");
    }

    public override void LoadStats<RangedWeaponStats>(RangedWeaponStats stats)
    {
        
    }
}
