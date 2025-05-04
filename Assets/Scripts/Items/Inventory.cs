using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    private List<InventoryItem> inventoryItems;
    public int Size { get; private set; } = 8;

    public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;
    public event Action<Item, int> onItemAdded;

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
    
    public void LoadFromData(InventoryData data)
    {
        foreach(InventoryItem item in data.inventoryItems)
        {
            if(item.item.itemData == null){
                Debug.LogWarning("Inventory item entry with NULL itemData");
                continue;
            }
            if (item.quantity <= 0){
                Debug.LogWarning($"Item {item.item.itemName} has quantity {item.quantity}. Skipping.");
                continue;
            }
            
            // Используем фабричный метод из ItemData для создания ПРАВИЛЬНОГО экземпляра Item
            Item newItemInstance = item.item.itemData.CreateItemInstance();

            // Добавляем созданный экземпляр в инвентарь
            AddItem(newItemInstance, item.quantity);
        }
    }

    public bool AddItem(Item itemToAdd, int quantity) // TODO: onItemAdded event for popUps
    {
        if (quantity > itemToAdd.itemData.maxStackSize){
            quantity = itemToAdd.itemData.maxStackSize;
            Debug.LogWarning($"Adding more {itemToAdd.itemName}'s than maxStackSize. Limiting quantity");
            return false;
        }
        
        // если нашли схожий предмет
        int foundSlotIndex = HasStackableItem(itemToAdd);
        if(foundSlotIndex != -1)  
        {
            InventoryItem itemToStack = new InventoryItem{item = itemToAdd, quantity = quantity};
            bool canStack = InventoryItem.CanStackCheck(itemToStack, inventoryItems[foundSlotIndex]);
            if (canStack)
            {
                InventoryItem sourceItemChanged;
                InventoryItem finalItem = InventoryItem.Stack(itemToStack, inventoryItems[foundSlotIndex], out sourceItemChanged);
                SetItemAt(foundSlotIndex, finalItem);
                
                onItemAdded?.Invoke(itemToAdd, quantity);
                InformAboutChange();
                return true;
            }
        }
        else
        {
            // Если не нашли похожий предмет
            if(AddToFirstEmptySlot(itemToAdd, quantity)){
                onItemAdded?.Invoke(itemToAdd, quantity);
                InformAboutChange();
                return true;
            }
        }
        
        //onItemAdded?.Invoke(itemToAdd, quantity);
        //InformAboutChange();
        return false;
    }
    
    public bool AddToFirstEmptySlot(Item itemToAdd, int quantity)
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

                return true;
            }
        }
        
        //Debug.LogWarning("[AddToFirstEmptySlot] Not enough space in inventory!");
        return false;
    }

    public void AddItem(InventoryItem item)
    {
        AddItem(item.item, item.quantity);
    }

    public Item DropItem(int index, Transform dropPos, int quantity = 1)
    {
        // for (int i = 0; i < quantity; i++)
        // {
        // }
        
        if (inventoryItems[index].quantity > 0)
        {
            WorldItem worldItem;
            worldItem = inventoryItems[index].item.SpawnItem(dropPos, quantity);
            worldItem.Drop();

            Item droppedItem = inventoryItems[index].item;
            inventoryItems[index] = inventoryItems[index].ChangeQuantity(inventoryItems[index].quantity - quantity);
            Debug.Log($"[{index}] Dropped {inventoryItems[index].item.itemName}");
            if (inventoryItems[index].quantity <= 0)
            {
                inventoryItems[index] = InventoryItem.GetEmptyItem();
            }
            
            InformAboutChange();
            return droppedItem;
        }
        else{
            Debug.Log($"[{index}] No item in slot");
        }
        
        //InformAboutChange();
        return null;
    }
    public Item DropWholeStack(int index, Transform dropPos)
    {
        if(inventoryItems[index].quantity > 0)
        {
            int startingQuantity = inventoryItems[index].quantity;
            
            WorldItem worldItem;
            worldItem = inventoryItems[index].item.SpawnItem(dropPos, startingQuantity);
            worldItem.Drop();
            
            
            Item droppedItem = inventoryItems[index].item;
            inventoryItems[index] = inventoryItems[index].ChangeQuantity(0);
            //Debug.Log($"[{index}] Dropped {inventoryItems[index].item.itemName}");
            if (inventoryItems[index].quantity <= 0)
            {
                inventoryItems[index] = InventoryItem.GetEmptyItem();
            }
            // здесь вместо itemPrefab спавнить нужный префаб если он есть
            // GameObject obj = GameObject.Instantiate(GameAssets.itemPrefab, dropPos.position, dropPos.rotation);
            // WorldItem worldItem = obj.GetComponent<WorldItem>();
            // worldItem.Initialize(inventoryItems[index].item.itemData, inventoryItems[index].item, startingQuantity);
            
            //inventoryItems[index] = inventoryItems[index].ChangeQuantity(inventoryItems[index].quantity - 1);

            InformAboutChange();
            return droppedItem;
        }
        else{
            //Debug.Log($"[DropWholeStack][{index}] No item in slot");
        }
        
        return null;
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Index of item if it is found. -1 is not found</returns>
    public int HasItem(InventoryItem item)
    {
        return inventoryItems.IndexOf(item);
    }
    
    public int HasItem(Item item)
    {
        int foundIndex = inventoryItems.FindIndex(x => !x.IsEmpty && x.item.itemData == item.itemData);
        // if(foundIndex >= 0){
        //     Debug.Log(inventoryItems[foundIndex].item.itemName + " found item");
        // }
        // else{
        //     Debug.Log($"[HasItem]: {item.itemName} not found in inventory");
        // }
        return foundIndex;
    }
    
    public int HasItem(ItemData itemData)
    {
        int foundIndex = inventoryItems.FindIndex(x => !x.IsEmpty && x.item.itemData == itemData);
        return foundIndex;
    }
    
    public int ItemAmount(ItemData itemData)
    {
        List<InventoryItem> foundItems = inventoryItems.FindAll(x => !x.IsEmpty && x.item.itemData == itemData);
        int count = 0;
        foreach (InventoryItem item in foundItems)
        {
            count += item.quantity;
        }
        return count;
    }
    
    // public bool CheckItemRequirements(ItemRequirements requirements)
    // {
    //     foreach(ItemRequirement req in requirements.requirements)
    //     {
    //         (Inventory _, int index) = HasItem(req.item);
    //         if(index != -1 && ItemAmount(req.item) > req.quantity)
    //         {
                
    //         }
    //     }
    // }
    
    public int HasStackableItem(Item item)
    {
        int foundIndex = inventoryItems.FindIndex(x => !x.IsEmpty && x.item.itemData == item.itemData && x.CanStack);
        // if(foundIndex >= 0)
        // {
        //     Debug.Log($"[HasStackableItem]: {inventoryItems[foundIndex].item.itemName} found item");
        // }
        // else{
        //     Debug.Log($"[HasStackableItem]: {item.itemName} not found in inventory");
        // }
        return foundIndex;
    }
    
    public bool HasItemAt(int index)
    {
        return !inventoryItems[index].IsEmpty;
    }


    public void DeleteItem(int index)
    {
        if(inventoryItems[index].IsEmpty){
            Debug.LogWarning($"No item found at index {index}");
            return;
        }
        
        if (!inventoryItems[index].IsEmpty && inventoryItems[index].IsStackable)
        {
            inventoryItems[index] = inventoryItems[index].SubtractQuantity();
        }
        else if (!inventoryItems[index].IsEmpty && !inventoryItems[index].IsStackable)
        {
            inventoryItems[index] = InventoryItem.GetEmptyItem();
        }
        
        InformAboutChange();
    }
    public bool DeleteItem(int index, int quantity)
    {
        if(inventoryItems[index].IsEmpty){
            Debug.LogWarning($"No item found at index {index}");
            return false;
        }
        
        int amountDeleted = 0;
        if (!inventoryItems[index].IsEmpty && inventoryItems[index].IsStackable)
        {
            for(int i = 0; i < quantity; i++){
                inventoryItems[index] = inventoryItems[index].SubtractQuantity();
                amountDeleted++;
            }
        }
        else if (!inventoryItems[index].IsEmpty && !inventoryItems[index].IsStackable)
        {
            amountDeleted++;
            inventoryItems[index] = InventoryItem.GetEmptyItem();
            Debug.LogWarning("Deleting NON stackable item with quantity arg");
        }
        
        //Debug.Log($"Amount deleted: {amountDeleted}");
        InformAboutChange();
        
        if(amountDeleted == quantity)
            return true;
        else
            return false;
    }
    public int DeleteAmount(int index, int quantity)
    {
        if(inventoryItems[index].IsEmpty){
            Debug.LogWarning($"No item found at index {index}");
            return -1;
        }
        
        int amountDeleted = 0;
        string itemName = inventoryItems[index].item.itemName;
        if (!inventoryItems[index].IsEmpty && inventoryItems[index].IsStackable)
        {
            for(int i = 0; i < quantity; i++){
                if(inventoryItems[index].quantity > 0){
                    inventoryItems[index] = inventoryItems[index].SubtractQuantity();
                    amountDeleted++;
                }
            }
        }
        else if (!inventoryItems[index].IsEmpty && !inventoryItems[index].IsStackable)
        {
            amountDeleted++;
            inventoryItems[index] = InventoryItem.GetEmptyItem();
            Debug.LogWarning("Deleting NON stackable item with quantity arg");
        }
        
        //Debug.Log($"Deleted {amountDeleted} {itemName}");
        InformAboutChange();
        
        return amountDeleted;
    }
    
    public void DeleteItem(InventoryItem item)
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
    
    public bool AllSlotsNotEmpty()
    {
        foreach(InventoryItem item in inventoryItems){
            if(item.IsEmpty)
            {
                return false;
            }
        }
        
        return true;
    }
    public int SpaceLeft()
    {
        if(!AllSlotsNotEmpty()){
            return 1;
        }
        
        int spaceLeft = 0;
        
        foreach(InventoryItem item in inventoryItems)
        {
            if(!item.IsEmpty){
                spaceLeft += item.MaxStackSize - item.quantity;
            }
        }
        
        return spaceLeft;
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
    
    public void SwapItems(int itemIndex_1, int itemIndex_2, InventoryItem from, InventoryItem to, int quantityToMove)
    {
        // Если пытаемся перетащить на тот же слот, ничего не делаем
        if (itemIndex_1 == itemIndex_2) return;

        // Если целевой слот пуст, просто перемещаем quantityToMove предметов
        if (to.IsEmpty)
        {
            // Уменьшаем количество в исходном слоте
            inventoryItems[itemIndex_1] = inventoryItems[itemIndex_1].ChangeQuantity(inventoryItems[itemIndex_1].quantity - quantityToMove);
            if (inventoryItems[itemIndex_1].quantity <= 0)
            {
                inventoryItems[itemIndex_1] = InventoryItem.GetEmptyItem();
            }

            // Создаем новый стак в целевом слоте
            inventoryItems[itemIndex_2] = new InventoryItem { item = from.item, quantity = quantityToMove };
        }
        // Если предметы одинаковые и можно стакать
        else if (InventoryItem.CanStackCheck(from, to))
        {
            int maxCanTake = to.MaxStackSize - to.quantity;
            int actualMoveAmount = Mathf.Min(quantityToMove, maxCanTake); // Сколько реально можем переместить

            if (actualMoveAmount <= 0) // Если целевой слот уже полон
            {
                // Если перетаскивали не весь стак (ПКМ), то ничего не делаем
                // Если перетаскивали весь стак (ЛКМ), то можно поменять местами, как раньше
                if (quantityToMove == inventoryItems[itemIndex_1].quantity) // Проверяем, был ли это полный драг
                {
                    InventoryItem item1 = inventoryItems[itemIndex_1];
                    inventoryItems[itemIndex_1] = inventoryItems[itemIndex_2];
                    inventoryItems[itemIndex_2] = item1;
                }
                // Иначе (ПКМ драг на полный слот того же типа) - ничего не делаем
                InformAboutChange(); // Вызываем на всякий случай, хотя изменений не было
                return;
            }


            // Уменьшаем количество в исходном слоте
            inventoryItems[itemIndex_1] = inventoryItems[itemIndex_1].ChangeQuantity(inventoryItems[itemIndex_1].quantity - actualMoveAmount);
            if (inventoryItems[itemIndex_1].quantity <= 0)
            {
                inventoryItems[itemIndex_1] = InventoryItem.GetEmptyItem();
            }

            // Увеличиваем количество в целевом слоте
            inventoryItems[itemIndex_2] = inventoryItems[itemIndex_2].ChangeQuantity(inventoryItems[itemIndex_2].quantity + actualMoveAmount);

        }
        // Если предметы разные ИЛИ одинаковые, но стакать нельзя/некуда (и это был полный драг)
        else if (quantityToMove == inventoryItems[itemIndex_1].quantity) // Только если перетаскивали весь стак
        {
            // Меняем местами, как и раньше
            InventoryItem item1 = inventoryItems[itemIndex_1];
            inventoryItems[itemIndex_1] = inventoryItems[itemIndex_2];
            inventoryItems[itemIndex_2] = item1;
        }
        // Если предметы разные и это был частичный драг (ПКМ), не позволяем обмен.
        // Можно добавить Debug.Log("Cannot split stack onto a different item type.");

        InformAboutChange();
    }

    public void InformAboutChange()
    {
        //Debug.Log($"Informing about change in {this}");
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
            //Debug.LogWarning("[Subtract Quant] GetEmptyItem");
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
            //Debug.Log($"Stacking complete. Dest: {destinationNewQuantity}/{finalItem.MaxStackSize}. Source emptied.");
        }
        else
        {
            // В источнике остались предметы
            sourceItemChanged = new InventoryItem{
                item = sourceItem.item,
                quantity = sourceNewQuantity
            };
            //Debug.Log($"Stacking partial. Dest: {destinationNewQuantity}/{finalItem.MaxStackSize}. Source left: {sourceNewQuantity}");
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
    
    public InventoryItem Half()
    {
        return new InventoryItem
        {
            item = item,
            quantity = Math.Clamp(Mathf.CeilToInt(this.quantity/2), 1, 1000)
        };
    }
    public int HalfQuantity()
    {
        return Math.Clamp(Mathf.CeilToInt(this.quantity/2), 1, 1000);
    }
}