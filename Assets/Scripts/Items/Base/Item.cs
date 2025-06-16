using System.Text;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public string description;
    public Sprite icon;
    public Rarity rarity;

    public ItemData itemData;

    public Item(ItemData data)
    {
        Init(data);
        LoadStats();
    }

    public virtual WorldItem SpawnItem(Vector3 spawnPos, Quaternion rotation, int quantity)
    {
        WorldItem worldItem;
        GameObject obj;
        if (itemData.customPrefab != null) // если есть кастомный префаб
        {
            obj = GameObject.Instantiate(itemData.customPrefab, spawnPos, rotation);
        }
        else
        {
            obj = GameObject.Instantiate(GameAssets.itemPrefab, spawnPos, rotation);
        }
        
        if(itemData.hasCustomScale){
            obj.transform.localScale = itemData.customScale;
        }
        
        worldItem = obj.GetComponent<WorldItem>();
        worldItem.Initialize(itemData, this, quantity);
        //worldItem.Drop();

        return worldItem;
    }
    public virtual WorldItem SpawnItem(Vector3 spawnPos, int quantity)
    {
        WorldItem worldItem;
        GameObject obj;
        if (itemData.customPrefab != null) // если есть кастомный префаб
        {
            obj = GameObject.Instantiate(itemData.customPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            obj = GameObject.Instantiate(GameAssets.itemPrefab, spawnPos, Quaternion.identity);
        }
        
        if(itemData.hasCustomScale){
            obj.transform.localScale = itemData.customScale;
        }
        worldItem = obj.GetComponent<WorldItem>();
        worldItem.Initialize(itemData, this, quantity);
        //worldItem.Drop();

        return worldItem;
    }

    public virtual void LoadStats() { }

    public virtual void Init<T>(T data)
    {
        // if (data is WeaponData weaponData)
        // {
        //     itemData = weaponData;
        // }
        if (data is MeleeWeaponData meleeWeaponData)
        {
            itemData = meleeWeaponData;
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

    public virtual string GetDescription()
    {
        StringBuilder str = new StringBuilder();
        str.AppendLine(description);
        return str.ToString();
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
    bool CanInteract();
    bool Interact();
}

public interface IEquipable
{
    void Equip();
    void Unequip();
}

// public interface IPickupable
// {
//     void Pickup();
//     void Drop();
// }

// public interface IStackable
// {
//     int MaxStackSize { get; }
// }

public interface IUsable
{
    void Use();
    bool CanUse();
}