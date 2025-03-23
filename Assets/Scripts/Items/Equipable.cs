using UnityEngine;

public abstract class Equipable : Item, IEquipable
{
    public virtual void Equip()
    {
        Debug.Log($"Item {itemName} equipped.");
    }

    public void Unequip()
    {
        Debug.Log($"Item {itemName} unequipped.");
    }
}
