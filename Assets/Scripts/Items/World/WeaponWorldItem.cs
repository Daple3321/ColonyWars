using UnityEngine;

public class WeaponWorldItem : WorldItem
{
    public float damage;
    public AttackType attackType;
    
    public override void Initialize(ItemData data, Item origin)
    {
        base.Initialize(data, origin);

        if (data is WeaponData weaponData)
        {
            damage = weaponData.damage;
            attackType = weaponData.attackType;
        }
    }

    public virtual void Attack(float concentraion)
    {
        
    }
}
