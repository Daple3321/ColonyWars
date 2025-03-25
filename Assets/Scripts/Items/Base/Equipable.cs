using UnityEngine;

[System.Serializable]
public class Equipable : Item, IEquipable
{

    public Equipable(ItemData _itemData) : base(_itemData)
    {
        Init(_itemData);
    }
    
    public virtual void Equip()
    {
        Debug.Log($"Item {itemName} equipped.");
    }

    public void Unequip()
    {
        Debug.Log($"Item {itemName} unequipped.");
    }
}
