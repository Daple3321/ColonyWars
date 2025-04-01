using UnityEngine;

[System.Serializable]
public class RangedWeapon : Weapon
{
    public float shootDistance;
    public float projectileSpeed;
    public ShootStyle shootStyle;
    
    public RangedWeapon(ItemData _itemData) : base(_itemData)
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
        if (attackType == AttackType.SINGLE && mouseReleased && _attackCd <= 0)
        {
            return true;
        }
        else if (attackType == AttackType.AUTOMATIC && _attackCd <= 0)
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
    }

    public override void LoadStats()
    {
        base.LoadStats();

        if (itemData is RangedWeaponData weaponData)
        {
            shootDistance = weaponData.shootDistance;
            shootStyle = weaponData.shootStyle;
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
