using UnityEngine;

[System.Serializable]
public class Weapon : Equipable
{
    public float damage;
    public float attackRate = 0.2f;
    protected float _attackCd;
    public float attackCharge = 0f;
    public float chargeRate;
    public AttackType attackType;
    public WeaponType weaponType;

    //public bool canAttack;
    public bool needsAmmo;
    public ItemData ammoType;
    public bool mouseReleased = true;
    
    public WeaponData weaponData;
    
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
            weaponType = data.weaponType;
            damage = data.damage;
            attackRate = data.attackRate;
            chargeRate = data.chargeRate;
            needsAmmo = data.needsAmmo;
            ammoType = data.ammoType;
            //Debug.Log($"LoadStats in Weapon.cs. WeaponData: {weaponData}");
            
            weaponData = data;
        }
    }

    public virtual void UpdateWeapon(){}
    
    public bool IsCharged() { return attackCharge >= 1; }
    
    public virtual void Charge()
    {
        attackCharge += chargeRate * Time.deltaTime;
    }
    
    public virtual bool CanAttack()
    {
        return true;
    }
    
    public virtual int GetAmmoInfo()
    {
        if(!needsAmmo) return 0;
        
        return GameController.p.playerInventory.ItemAmount(ammoType);
    }
    public virtual bool CheckAmmo()
    {
        if(needsAmmo)
        {
            (Inventory inv, int index) = GameController.p.playerInventory.HasItem(ammoType);
            if(index != -1)
            {
                //Debug.Log($"Has ammo: {inv.GetItemAt(index).item.itemName}, {inv.GetItemAt(index).quantity}");
                return true;
            }
            else{
                return false;
            }
        }
        else{
            return true;
        }
    }
    public virtual void SubtractAmmo()
    {
        if(!needsAmmo) return;
        
        (Inventory inv, int index) = GameController.p.playerInventory.HasItem(ammoType);
        inv.DeleteItem(index);
    }
    
    public virtual void Attack()
    {
        Debug.Log($"Attacked with {itemName}");
    }
}

public enum WeaponType : byte
{
    None,
    Pistol,
    Rifle,
    Sword,
    Knife,
}

public enum AttackType : byte
{
    NONE,
    AUTOMATIC, // если зажать кнопку - само будет атаковать по кд
    SINGLE, // если зажать кнопку - будет только одна атака. Надо каждый раз кликать.
    CHARGE, // при зажатии - начинается чардж (атака только после полного чарджа). Сброс при отжатии

}
