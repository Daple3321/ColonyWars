using UnityEngine;

public class WeaponData : ItemData
{
    public float damage;
    public float attackRate = 0.2f;
    public bool needsAmmo;
    public ItemData ammoType;

    public AttackType attackType;
    
    public WeaponType weaponType;
}
