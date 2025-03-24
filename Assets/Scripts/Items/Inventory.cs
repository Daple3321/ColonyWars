using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    private List<InventoryItem> inventoryItems;
    public int Size { get; private set; } = 8;

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
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty)
            {
                inventoryItems[i] = new InventoryItem
                {
                    item = itemToAdd,
                    quantity = quantity,
                };

                break;
            }
        }
    }

    public void DropItem(int index, Transform dropPos, int quantity = 1 )
    {
        for (int i = 0; i < quantity; i++)
        {
            if (inventoryItems[index].quantity > 0)
            {
                // здесь вместо itemPrefab спавнить нужный префаб если он есть
                GameObject obj = GameObject.Instantiate(GameAssets.itemPrefab, dropPos.position, dropPos.rotation);
                WorldItem worldItem = obj.GetComponent<WorldItem>();
                worldItem.Initialize(inventoryItems[index].item.itemData, inventoryItems[index].item);
                
                inventoryItems[index] = inventoryItems[index].ChangeQuantity(inventoryItems[index].quantity - 1);
                Debug.Log($"[{index}] Dropped {inventoryItems[index].item.itemName}");
            }
            else
            {
                Debug.Log("Trying to drop more than available quantity!");
            }
        }
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

    public Item GetItem(int index)
    {
        if (!inventoryItems[index].IsEmpty)
            return inventoryItems[index].item;
        else
        {
            return null; // ???
        }
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
                Debug.Log($"[{i}] Item: {inventoryItems[i].item.itemName}, Quantity: {inventoryItems[i].quantity}");
            }
            else
            {
                Debug.Log($"[{i}] Slot empty");
            }
        }
    }

    // public int FindEmptySlot()
    // {
    //     return inventory.LastIndexOf(null);
    // }

    // public bool HasSpace()
    // {
    //     if (inventory.Count < inventory.Capacity)
    //     {
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }
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