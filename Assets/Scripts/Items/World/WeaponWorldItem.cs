using Unity.Cinemachine;
using UnityEngine;

public class WeaponWorldItem : WorldItem
{
    public float damage;
    public AttackType attackType;
    
    public CinemachineImpulseSource impulseSource;
    
    public override void Initialize(ItemData data, Item origin, int quantity=1)
    {
        base.Initialize(data, origin, quantity);

        if (data is WeaponData weaponData)
        {
            damage = weaponData.damage;
            attackType = weaponData.attackType;
        }
    }

    public virtual void Attack(float concentraion)
    {
        if(impulseSource != null){
            impulseSource.GenerateImpulse();
        }
    }
}
