using UnityEngine;

public class WeaponWorldItem : WorldItem
{
    public override void Initialize(ItemData data, Item origin)
    {
        base.Initialize(data, origin);

        if (data is WeaponData weaponData)
        {
            
        }
    }

    public virtual void Attack(float concentraion)
    {
        
    }
}
