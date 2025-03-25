using UnityEngine;

[System.Serializable]
public class MeleeWeapon : Weapon
{
    public float attackDuration;
    public float attackCharge;
    public float attackInterval;

    public MeleeWeapon(ItemData _itemData) : base(_itemData)
    {
        Init(_itemData);
    }

    public override void LoadStats()
    {
        base.LoadStats();
        
        if (itemData is MeleeWeaponData weaponData)
        {
            attackDuration = weaponData.attackDuration;
            attackCharge = weaponData.attackCharge;
            attackInterval = weaponData.attackInterval;
        }
    }
}
