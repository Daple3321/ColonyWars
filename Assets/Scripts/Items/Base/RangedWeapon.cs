using UnityEngine;

[System.Serializable]
public class RangedWeapon : Weapon
{
    public float shootDistance;
    public float projectileSpeed;
    public int penetrationAmount;
    public float projectileLifetime;
    public int projectilesPerShot;
    public ShootStyle shootStyle;
    
    public RangedWeapon(RangedWeaponData _itemData) : base(_itemData)
    {
        Init(_itemData);
    }

    public override void UpdateWeapon()
    {
        if (_attackCd > 0)
        {
            _attackCd -= Time.deltaTime;
        }
    }
    
    public override bool CanAttack()
    {
        if (attackType == AttackType.SINGLE && mouseReleased && _attackCd <= 0 && CheckAmmo())
        {
            return true;
        }
        else if (attackType == AttackType.AUTOMATIC && _attackCd <= 0 && CheckAmmo())
        {
            return true;
        }
        else if(attackType == AttackType.CHARGE && _attackCd <= 0 && IsCharged() && CheckAmmo())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    

    public override void Attack()
    {
        _attackCd = attackRate;
        attackCharge = 0;
        if(needsAmmo)
        {
            SubtractAmmo();
            Debug.Log($"Ammo count: {GetAmmoInfo()}");
        }
    }

    public override void LoadStats()
    {
        base.LoadStats();

        if (itemData is RangedWeaponData weaponData)
        {
            shootDistance = weaponData.shootDistance;
            shootStyle = weaponData.shootStyle;
            projectileLifetime = weaponData.projectileLifetime;
            penetrationAmount = weaponData.penetrationAmount;
            projectilesPerShot = weaponData.projectilesPerShot;
        }

        _attackCd = attackRate;
    }
}

public enum ShootStyle : byte
{
    HITSCAN,
    PROJECTILE,
    AREA_HITSCAN,
}
