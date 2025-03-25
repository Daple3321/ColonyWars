using UnityEngine;

[System.Serializable]
public class RangedWeapon : Weapon
{
    public float shootDistance;
    public float shootInterval;


    // public override void LoadStats<RangedWeaponData>(RangedWeaponData stats)
    // {
    //     weaponData = stats;
    // }
    
    public RangedWeapon(ItemData _itemData) : base(_itemData)
    {
        Init(_itemData);
    }
    
    public override void LoadStats()
    {
        base.LoadStats();
        
        if (itemData is RangedWeaponData weaponData)
        {
            shootDistance = weaponData.shootDistance;
            shootInterval = weaponData.shootInterval;
        }
    }
}

