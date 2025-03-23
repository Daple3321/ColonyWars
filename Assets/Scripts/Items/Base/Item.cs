using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Item
{
    public string itemName;
    public string description;
    public Sprite icon;
    public Rarity rarity;

    public ItemData itemData;

    public Item(ItemData _itemData)
    {
        Init(_itemData);
        LoadStats();
    }
    
    public virtual void LoadStats(){}
    
    public virtual void Init<T>(T data)
    {
        if (data is MeleeWeaponData weaponData)
        {
            itemData = weaponData;
        }
        if (data is RangedWeaponData rangedWeaponData)
        {
            itemData = rangedWeaponData;
        }
        if (data is ItemData _itemData)
        {
            itemData = _itemData;
            itemName = _itemData.itemName;
            description = _itemData.description;
            icon = _itemData.icon;
            rarity = _itemData.rarity;
        }
    }
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