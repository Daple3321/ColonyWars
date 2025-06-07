using Unity.Cinemachine;
using UnityEngine;

public class WeaponWorldItem : WorldItem
{
    [Space(7), Header("Weapon Settings")]
    public float damage;
    public float slowingAmount;
    public AttackType attackType;
    
    protected Weapon originWeapon;
    public CinemachineImpulseSource impulseSource;
    protected Camera cm;
    public override void Initialize(ItemData data, Item origin, int quantity=1)
    {
        base.Initialize(data, origin, quantity);
        
        if(origin is Weapon wp){
            this.originWeapon = wp;
        }

        if (data is WeaponData weaponData)
        {
            damage = weaponData.damage;
            attackType = weaponData.attackType;
            slowingAmount = weaponData.slowingAmount;
        }
    }

    public virtual void Attack(float concentraion)
    {
        if(impulseSource != null){
            impulseSource.GenerateImpulse();
        }
    }
}
