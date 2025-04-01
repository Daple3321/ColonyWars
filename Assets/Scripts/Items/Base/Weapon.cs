using UnityEngine;

[System.Serializable]
public class Weapon : Equipable
{
    public float damage;
    public float attackRate = 0.2f;
    protected float _attackCd;
    public AttackType attackType;

    //public bool canAttack;
    public bool mouseReleased = true;
    
    public Weapon(ItemData _itemData) : base(_itemData)
    {
        Init(_itemData);
        //LoadStats();
    }

    public override void LoadStats()
    {
        base.LoadStats();

        if (itemData is WeaponData data)
        {
            attackType = data.attackType;
            damage = data.damage;
            attackRate = data.attackRate;
            //Debug.Log($"LoadStats in Weapon.cs. WeaponData: {weaponData}");
        }
    }

    public virtual void UpdateWeapon(){}

    public virtual bool CanAttack()
    {
        return true;
    }
    
    public virtual void Attack()
    {
        Debug.Log($"Attacked with {itemName}");
    }
}

public enum AttackType : byte
{
    NONE,
    AUTOMATIC, // если зажать кнопку - само будет атаковать по кд
    SINGLE, // если зажать кнопку - будет только одна атака. Надо каждый раз кликать.
    CHARGE, // при зажатии - начинается чардж (атака только после полного чарджа). Сброс при отжатии

}
