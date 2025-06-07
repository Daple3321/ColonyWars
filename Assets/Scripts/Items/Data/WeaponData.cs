using UnityEngine;

public class WeaponData : ItemData
{
    [Space(10), Header("Weapon Data")]
    public float damage;
    public float attackRate = 0.2f;
    public float chargeRate = 0.1f;
    public bool needsAmmo;
    public float recoilForce = 0f;
    public float slowingAmount = 0.5f;
    public ItemData ammoType;

    public AttackType attackType;
    
    public WeaponType weaponType;
}
