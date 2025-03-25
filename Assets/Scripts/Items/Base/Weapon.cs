using UnityEngine;

[System.Serializable]
public class Weapon : Equipable
{
    public WeaponData weaponData;

    public Weapon(ItemData _itemData) : base(_itemData)
    {
        Init(_itemData);
    }

    public virtual void Attack()
    {
        Debug.Log($"Attacked with {itemName}");
    }

    //public abstract void LoadStats<T>(T stats);
}