using UnityEngine;

[System.Serializable]
public class Weapon : Equipable
{
    public WeaponData weaponData;

    public Weapon(ItemData _itemData) : base(_itemData)
    {
        Init(_itemData);
    }

    //public abstract void LoadStats<T>(T stats);
}