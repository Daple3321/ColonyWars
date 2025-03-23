using UnityEngine;

public class MeleeWeapon : Weapon
{
    public override void Attack()
    {
        Debug.Log($"Melee attack with: {itemName}");
    }

    public override void LoadStats<MeleeWeaponStats>(MeleeWeaponStats stats)
    {
        
    }
}
