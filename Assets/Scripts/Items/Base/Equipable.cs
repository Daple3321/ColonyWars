using UnityEngine;

[System.Serializable]
public abstract class Equipable : Item, IEquipable
{
    public virtual void Equip()
    {
        Debug.Log($"Item {itemName} equipped.");
    }

    public Equipable(ItemData _itemData) : base(_itemData)
    {
        Init(_itemData);
    }

    public void Unequip()
    {
        Debug.Log($"Item {itemName} unequipped.");
    }
}
