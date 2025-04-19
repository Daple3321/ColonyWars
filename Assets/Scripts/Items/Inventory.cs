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
    
    public void SetItemAt(int index, InventoryItem item)
    {
        if (index >= 0 && index < inventoryItems.Count)
        {
            inventoryItems[index] = item;
        }
        else
        {
            Debug.LogError($"SetItemAt: Index {index} out of bounds for inventory size {Size}");
        }
    }   
    
    public bool HasItemAt(int index)
    {
        return !inventoryItems[index].IsEmpty;
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
    public void DestroyItem(InventoryItem item)
    {
        int itemIndex = inventoryItems.IndexOf(item);
        if (itemIndex == -1)
        {
            Debug.Log($"Can't find item {item.item.itemName} to delete");
            return;
        }

        inventoryItems[itemIndex] = InventoryItem.GetEmptyItem();
        InformAboutChange();
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
        bool canStack = InventoryItem.CanStackCheck(from, to);
        if (canStack)
        {
            InventoryItem sourceItemChanged;
            InventoryItem finalItem = InventoryItem.Stack(from, to, out sourceItemChanged);
            
            inventoryItems[itemIndex_1] = sourceItemChanged;
            inventoryItems[itemIndex_2] = finalItem;
        }
        else
        {
            InventoryItem item1 = inventoryItems[itemIndex_1];
            inventoryItems[itemIndex_1] = inventoryItems[itemIndex_2];
            inventoryItems[itemIndex_2] = item1;
        }

        InformAboutChange();
    }

    public void InformAboutChange()
    {
        Debug.Log($"Informing about change in {this}");
        OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
    }
}

[System.Serializable]
public struct InventoryItem
{
    public int quantity;
    public Item item;

    public bool IsEmpty => item == null;

    public bool CanStack => quantity < MaxStackSize && IsStackable;
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

    public static bool CanStackCheck(InventoryItem from, InventoryItem to)
    {
        //int maxAvailableTransfer;
        //maxAvailableTransfer = from.MaxStackSize - to.quantity;
        return !to.IsEmpty && from.item.itemData == to.item.itemData && to.CanStack;
    }
    
    public static InventoryItem Stack(InventoryItem sourceItem, InventoryItem destinationItem,
        out InventoryItem sourceItemChanged)
    {
        InventoryItem finalItem = destinationItem;
        sourceItemChanged = sourceItem;
        
        if (sourceItem.IsEmpty ||
            destinationItem.IsEmpty ||
            sourceItem.item.itemData != destinationItem.item.itemData ||
            destinationItem.quantity >= destinationItem.MaxStackSize) // Цель уже полная?
        {
            Debug.LogWarning($"Stacking failed");
            return finalItem;
        }
        
        int maxAvailableTransfer = destinationItem.MaxStackSize - destinationItem.quantity;
        int amountToMove = Mathf.Min(sourceItem.quantity, maxAvailableTransfer);

        if (amountToMove <= 0)
        {
            return finalItem;
        }
        
        int sourceNewQuantity = sourceItem.quantity - amountToMove;
        int destinationNewQuantity = destinationItem.quantity + amountToMove;

        // 4. Формирование итоговых состояний слотов
        finalItem = new InventoryItem{
            item = destinationItem.item,
            quantity = destinationNewQuantity,
        };
        
        if (sourceNewQuantity <= 0)
        {
            // Источник полностью опустел
            sourceItemChanged = GetEmptyItem(); // Используем GetEmpty() для создания "пустого" предмета
            Debug.Log($"Stacking complete. Dest: {destinationNewQuantity}/{finalItem.MaxStackSize}. Source emptied.");
        }
        else
        {
            // В источнике остались предметы
            sourceItemChanged = new InventoryItem{
                item = sourceItem.item,
                quantity = sourceNewQuantity
            };
            Debug.Log($"Stacking partial. Dest: {destinationNewQuantity}/{finalItem.MaxStackSize}. Source left: {sourceNewQuantity}");
        }
        
        /* логика стакинга */
        // if (sourceItem.quantity == maxAvailableTransfer) // 5/8 + 3/8 = 8/8
        // {
        //     int moveAmount = destinationItem.quantity + maxAvailableTransfer;
        //     sourceItemChanged = GetEmptyItem();
        //     finalItem = new InventoryItem
        //     {
        //         quantity = moveAmount,
        //         item = sourceItem.item,
        //     };
        //     Debug.Log($"Stacking to maxAmount {moveAmount}");
        // }
        // else if (sourceItem.quantity == destinationItem.quantity) // 5/10 + 5/10 = 10/10 <-- НЕ РАБОТАЕТ???
        // {
        //     int moveAmount = destinationItem.quantity + maxAvailableTransfer;
        //     sourceItemChanged = GetEmptyItem();
        //     finalItem = new InventoryItem
        //     {
        //         quantity = moveAmount,
        //         item = sourceItem.item,
        //     };
        // }
        // else if (sourceItem.quantity < maxAvailableTransfer) // 3/10 + 5/10 = 8/10
        // {
        //     int moveAmount = sourceItem.quantity + destinationItem.quantity;
        //     sourceItemChanged = GetEmptyItem();
        //     finalItem = new InventoryItem
        //     {
        //         quantity = moveAmount,
        //         item = sourceItem.item,
        //     };
        //     Debug.Log($"Stacking, deleting sourceItem completly {moveAmount}");
        // }
        // else if (sourceItem.quantity > maxAvailableTransfer) // 8/10 + 8/10 = [10/10; 6/10]
        // {
        //     int moveAmount = maxAvailableTransfer + destinationItem.quantity;
        //     sourceItemChanged = new InventoryItem
        //     {
        //         quantity = sourceItem.quantity - maxAvailableTransfer,
        //         item = sourceItem.item,
        //     };
        //     finalItem = new InventoryItem
        //     {
        //         quantity = moveAmount,
        //         item = sourceItem.item,
        //     };
        // }
        
        return finalItem;
    }
    
    public static InventoryItem GetEmptyItem()
    => new InventoryItem
    {
        item = null,
        quantity = 0,
    };
}