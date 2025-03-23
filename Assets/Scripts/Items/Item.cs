using UnityEngine;

public abstract class Item
{
    public string itemName;
    public string description;
    public Sprite icon;
    public Rarity rarity;
}

public enum Rarity
{
    None,
    Common,
    Rare,
    VeryRare,
    Legendary,
    Mythical,
}

public interface IInteractable
{
    void CanInteract();
    void Interact();
}

public interface IEquipable
{
    void Equip();
    void Unequip();
}

public interface IPickupable
{
    void Pickup();
    void Drop();
}

public interface IStackable
{
    int MaxStackSize { get; }
}

public interface IUsable
{
    void Use();
    bool CanUse();
}