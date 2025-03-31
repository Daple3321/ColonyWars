using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    private List<InventoryItem> inventoryItems;
    public int Size { get; private set; } = 8;

    public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

    public Inventory(int size)
    {
        Size = size;
        InitInventory(size);
    }

    public void InitInventory(int size)
    {
        inventoryItems = new List<InventoryItem>();
        inventoryItems.Capacity = size;
        for (int i = 0; i < Size; i++)
        {
            inventoryItems.Add(InventoryItem.GetEmptyItem());
        }

    }

    public void AddItem(Item itemToAdd, int quantity)
    {
        if (quantity > itemToAdd.itemData.maxStackSize)
        {
            quantity = itemToAdd.itemData.maxStackSize;
            Debug.Log($"Adding more {itemToAdd.itemName}'s than maxStackSize. Limiting quantity");
        }

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty)
            {
                inventoryItems[i] = new InventoryItem
                {
                    item = itemToAdd,
                    quantity = quantity,
                };

                return;
            }
        }

        InformAboutChange();
    }

    public void AddItem(InventoryItem item)
    {
        AddItem(item.item, item.quantity);
    }

    public Item DropItem(int index, Transform dropPos, int quantity = 1)
    {
        for (int i = 0; i < quantity; i++)
        {
            if (inventoryItems[index].quantity > 0)
            {
                WorldItem worldItem;
                worldItem = inventoryItems[index].item.SpawnItem(dropPos);

                Item droppedItem = inventoryItems[index].item;
                inventoryItems[index] = inventoryItems[index].ChangeQuantity(inventoryItems[index].quantity - 1);
                Debug.Log($"[{index}] Dropped {inventoryItems[index].item.itemName}");
                if (inventoryItems[index].quantity <= 0)
                {
                    inventoryItems[index] = InventoryItem.GetEmptyItem();
                }
                
                InformAboutChange();
                return droppedItem;
            }
            else
            {
                Debug.Log($"[{index}] No item in slot");
            }
        }
        //InformAboutChange();
        return null;
    }
    public void DropWholeStack(int index, Transform dropPos)
    {
        int startingQuantity = inventoryItems[index].quantity;
        for (int i = 0; i < startingQuantity; i++)
        {
            // здесь вместо itemPrefab спавнить нужный префаб если он есть
            GameObject obj = GameObject.Instantiate(GameAssets.itemPrefab, dropPos.position, dropPos.rotation);
            WorldItem worldItem = obj.GetComponent<WorldItem>();
            worldItem.Initialize(inventoryItems[index].item.itemData, inventoryItems[index].item);

            inventoryItems[index] = inventoryItems[index].ChangeQuantity(inventoryItems[index].quantity - 1);
        }
        InformAboutChange();
    }
    

    public Dictionary<int, InventoryItem> GetCurrentInventoryState()
    {
        Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty)
                continue;
            returnValue[i] = inventoryItems[i];
        }

        return returnValue;
    }

    public InventoryItem GetItemAt(int index)
    {
        if (!inventoryItems[index].IsEmpty)
            return inventoryItems[index];
        else
        {
            return InventoryItem.GetEmptyItem(); // ???
        }
    }
    
    public bool HasItemAt(int index)
    {
        return !inventoryItems[index].IsEmpty;
    }

    public Item FindItem(ItemData itemData)
    {
        return null;
    }

    public Item FindItem(string itemName)
    {
        return null;
    }

    public void DeleteItem(int index)
    {
        if (!inventoryItems[index].IsEmpty && inventoryItems[index].IsStackable)
        {
            inventoryItems[index] = inventoryItems[index].SubtractQuantity();
        }
        else if (!inventoryItems[index].IsEmpty && !inventoryItems[index].IsStackable)
        {
            inventoryItems[index] = InventoryItem.GetEmptyItem();
        }
        else if (inventoryItems[index].IsEmpty)
        {
            Debug.Log($"No item found at index {index}");
        }
    }
    public void DeleteItem(Item itemToDelete)
    {
        
    }

    public void PrintInv()
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (!inventoryItems[i].IsEmpty)
            {
                Debug.Log($"[{i}] {inventoryItems[i].item.itemName}; ({inventoryItems[i].quantity})");
            }
            else
            {
                Debug.Log($"[{i}] Slot empty");
            }
        }
    }

    public void SwapItems(int itemIndex_1, int itemIndex_2, InventoryItem from, InventoryItem to)
    {
        // if (inventoryItems[itemIndex_1].IsEmpty || inventoryItems[itemIndex_2].IsEmpty) // чёёё
        //     return;
        
        InventoryItem item1 = inventoryItems[itemIndex_1];
        inventoryItems[itemIndex_1] = inventoryItems[itemIndex_2];
        inventoryItems[itemIndex_2] = item1;
        InformAboutChange();
    }

    private void InformAboutChange()
    {
        OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
    }
}

[System.Serializable]
public struct InventoryItem
{
    public int quantity;
    public Item item;

    public bool IsEmpty => item == null;

    public bool IsStackable => item.itemData.IsStackable;
    public int MaxStackSize => item.itemData.maxStackSize;

    public InventoryItem ChangeQuantity(int newQuantity)
    {
        return new InventoryItem
        {
            quantity = newQuantity,
            item = this.item,
        };
    }

    public InventoryItem SubtractQuantity() // бред больного
    {
        if (quantity >= 2)
        {
            return new InventoryItem
            {
                quantity = quantity - 1,
                item = this.item,
            };
        }
        else
        {
            return GetEmptyItem();
        }
    }

    public static InventoryItem GetEmptyItem()
    => new InventoryItem
    {
        item = null,
        quantity = 0,
    };
}