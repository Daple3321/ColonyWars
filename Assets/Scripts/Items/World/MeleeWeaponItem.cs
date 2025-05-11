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
    
    public override void Attack(float concentraion) // ВЫСТРЕЛЫ ПРОСТО НЕ СПАВНЯТСЯ ЕСЛИ НИЧЕГО НЕ ХИТАНУЛИ.
    {
        base.Attack(concentraion);
        
        
    }
}
