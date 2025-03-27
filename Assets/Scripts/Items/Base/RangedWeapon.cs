using UnityEngine;

[System.Serializable]
public class RangedWeapon : Weapon
{
    public float shootDistance;
    public float shootInterval;

    public ShootStyle shootStyle;

    // public override void LoadStats<RangedWeaponData>(RangedWeaponData stats)
    // {
    //     weaponData = stats;
    // }

    public RangedWeapon(ItemData _itemData) : base(_itemData)
    {
        Init(_itemData);
        //LoadStats();
    }
    
    public override void LoadStats()
    {
        base.LoadStats();

        if (itemData is RangedWeaponData weaponData)
        {
            shootDistance = weaponData.shootDistance;
            shootInterval = weaponData.shootInterval;
            shootStyle = weaponData.shootStyle;
        }
    }
}

public enum ShootStyle : byte
{
    HITSCAN,
    PROJECTILE,
    AREA_HITSCAN,
}
