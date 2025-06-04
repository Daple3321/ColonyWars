using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory inventory;
    public int inventoryStartingSize = 8;
    
    public Inventory hotbar;
    public int hotbarStartingSize = 4;
    
    public List<ItemData> itemDatas = new List<ItemData>();

    public GameObject itemPrefab;

    public int selectedSlotId = -1;
    [SerializeReference] public Item selectedItem = null;
    public WorldItem selectedWorldItem;

    private Controls controls;
    
    public InventoryData hotbarStartInventory;
    public InventoryData starterInventory;
    
    private Player player;
    [SerializeField] private Transform dropPoint;
    private Transform rightHand;
    private Transform leftHand;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private InventoryUI hotbarUI;
    void Awake()
    {
        enabled = false;
    }
    public void Init(Player player)
    {
        this.player = player;
        rightHand = player.rightHand;
        leftHand = player.leftHand;
        controls = GameAssets.controls;
        enabled = true;

        DragDropManager.Init();

        inventory = new Inventory(inventoryStartingSize);
        inventory.OnInventoryUpdated += UpdateUI;


        hotbar = new Inventory(hotbarStartingSize);
        hotbar.OnInventoryUpdated += UpdateHotbarUI;
        hotbar.OnInventoryUpdated += UpdateSelectedItem;
        
        inventory.OnInventoryUpdated += OnInventoryUpdated;
        hotbar.OnInventoryUpdated += OnInventoryUpdated;
        
        PrepareUI();

        //inventory.AddItem(new RangedWeapon(itemDatas[1]), 1);
        //inventory.AddItem(new Item(itemDatas[2]), 5);
        //inventory.AddItem(new MeleeWeapon(itemDatas[0]), 1);
        //inventory.AddItem(new Item(itemDatas[2]), 1);
        //inventory.AddItem(new RangedWeapon(itemDatas[3]), 1);
        //inventory.AddItem(new RangedWeapon(itemDatas[4]), 1);
        
        hotbar.LoadFromData(hotbarStartInventory);
        inventory.LoadFromData(starterInventory);
        //hotbar.LoadFromData(starterInventory);
        
        
        //hotbar.AddItem(new RangedWeapon(itemDatas[1]), 1);
        //hotbar.AddItem(new MeleeWeapon(itemDatas[0]), 1);
        //hotbar.AddItem(new Item(itemDatas[3]), 5);
        //hotbar.AddItem(new Item(itemDatas[4]), 3);
        //SelectItem(hotbar, selectedSlotId);
    }

    private void UpdateUI(Dictionary<int, InventoryItem> inventoryState)
    {
        inventoryUI.ResetAllItems();
        foreach (var item in inventoryState)
        {
            inventoryUI.UpdateData(item.Key, item.Value.item.icon, item.Value.quantity, item.Value);
        }
    }

    private void UpdateHotbarUI(Dictionary<int, InventoryItem> inventoryState)
    {
        hotbarUI.ResetAllItems();
        foreach (var item in inventoryState)
        {
            hotbarUI.UpdateData(item.Key, item.Value.item.icon, item.Value.quantity, item.Value);
        }
    }

    private void PrepareUI()
    {
        GameObject invObj = Instantiate(GameAssets.inventoryUI_Prefab, GameController.i.mainCanvas.transform);
        inventoryUI = invObj.GetComponent<InventoryUI>();
        inventoryUI.InitializeInventoryUI(inventoryStartingSize);
        this.inventoryUI.OnSwapItems += HandleSwapItems;
        this.inventoryUI.OnStartDragging += HandleDragging;
        this.inventoryUI.OnItemVoidDrop += HandleVoidDrop;
        this.inventoryUI.OnItemActionRequested += HandleItemActionRequest;
        this.inventoryUI.OnTransferItemsRequest += HandleTransferRequest;
        inventoryUI.SetLinkedInventory(inventory);

        GameObject hotbarObj = Instantiate(GameAssets.hotbarUI_Prefab, GameController.i.mainCanvas.transform);
        hotbarUI = hotbarObj.GetComponent<InventoryUI>();
        hotbarUI.InitializeInventoryUI(hotbarStartingSize);
        this.hotbarUI.OnSwapItems += HandleSwapItemsHotbar;
        this.hotbarUI.OnStartDragging += HandleDraggingHotbar;
        this.hotbarUI.OnItemVoidDrop += HandleVoidDropHotbar;
        this.hotbarUI.OnItemActionRequested += HandleItemActionRequest;
        this.hotbarUI.OnTransferItemsRequest += HandleTransferRequest;
        hotbarUI.SetLinkedInventory(hotbar);
        foreach (var item in hotbar.GetCurrentInventoryState())
        {
            hotbarUI.UpdateData(item.Key, item.Value.item.icon, item.Value.quantity, item.Value);
        }

        GameController.i.mouseFollower.transform.SetAsLastSibling();
    }

    private void HandleItemActionRequest(int itemIndex)
    {
        
    }
    
    private void HandleVoidDrop(int itemIndex, int quantity)
    {
        HandleVoidDropInternal(inventory, itemIndex, quantity);
    }
    private void HandleVoidDropHotbar(int itemIndex, int quantity)
    {
        HandleVoidDropInternal(hotbar, itemIndex, quantity);
    }
    private void HandleVoidDropInternal(Inventory sourceInv, int itemIndex, int quantity)
    {
        if(quantity == -1){
            sourceInv.DropWholeStack(itemIndex, dropPoint.position, dropPoint.forward);
        }
        else{
            sourceInv.DropItem(itemIndex, dropPoint.position, dropPoint.forward, quantity);
        }
    }

    private void HandleDragging(int itemIndex, int dragQuantity)
    {
        HandleDraggingInternal(inventory, inventoryUI, itemIndex, dragQuantity);
    }
    private void HandleDraggingHotbar(int itemIndex, int dragQuantity)
    {
        HandleDraggingInternal(hotbar, hotbarUI, itemIndex, dragQuantity);
    }

    private void HandleDraggingInternal(Inventory sourceInv, InventoryUI sourceUI, int itemIndex, int dragQuantity)
    {
        InventoryItem inventoryItem = sourceInv.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;
        
        if(dragQuantity == -1){ // взять полностью весь стак
            sourceUI.CreateDraggedItem(inventoryItem.item.icon, inventoryItem.quantity, inventoryItem);
        }
        else{
            sourceUI.CreateDraggedItem(inventoryItem.item.icon, dragQuantity, inventoryItem);
        }
    }

    private void HandleSwapItems(int itemIndex_1, int itemIndex_2, InventoryItem from, InventoryItem to)
    {
        //Debug.Log($"Swapping: {from.item.itemName} with {to.item.itemName}");
        HandleSwapInternal(inventory, itemIndex_1, itemIndex_2, from, to);
    }
    private void HandleSwapItemsHotbar(int itemIndex_1, int itemIndex_2, InventoryItem from, InventoryItem to)
    {
        //Debug.Log($"Swapping: {from.item.itemName} with {to.item.itemName}");
        HandleSwapInternal(hotbar, itemIndex_1, itemIndex_2, from, to);
    }

    public void HandleSwapInternal(Inventory sourceInv, int itemIndex_1, int itemIndex_2, InventoryItem from, InventoryItem to)
    {
        // InventoryItem fromModified = new InventoryItem{
        //     item = from.item,
        //     quantity = DragDropManager.dragQuantity
        // };
        sourceInv.SwapItems(itemIndex_1, itemIndex_2, from, to, DragDropManager.dragQuantity);
    }
    
    public void HandleTransferRequest(InventoryUI sourceUI, int sourceIndex, InventoryUI destinationUI, int destinationIndex)
    {
        Inventory sourceInventory = sourceUI.LinkedInventory;
        Inventory destinationInventory = destinationUI.LinkedInventory;

        if (sourceInventory == null || destinationInventory == null)
        {
            Debug.LogError("Transfer failed: Linked inventory not found.");
            return;
        }

        InventoryItem itemToMove = sourceInventory.GetItemAt(sourceIndex);
        if (itemToMove.IsEmpty)
        {
            Debug.LogWarning("Transfer failed: Source slot is empty.");
            return;
        }

        InventoryItem itemAtDestination = destinationInventory.GetItemAt(destinationIndex);
        int quantityToMove = DragDropManager.dragQuantity; // Получаем количество для переноса


        // --- Логика переноса/обмена между инвентарями ---

        // 1. Простой случай: перемещение в пустой слот назначения
        if (itemAtDestination.IsEmpty)
        {
            //sourceInventory.SetItemAt(sourceIndex, InventoryItem.GetEmptyItem()); // Очищаем источник
            //destinationInventory.SetItemAt(destinationIndex, itemToMove);       // Помещаем в назначение
            
            // Уменьшаем количество в источнике
            InventoryItem sourceRemaining = itemToMove.ChangeQuantity(itemToMove.quantity - quantityToMove);
            if (sourceRemaining.quantity <= 0)
            {
                sourceInventory.SetItemAt(sourceIndex, InventoryItem.GetEmptyItem());
            }
            else
            {
                sourceInventory.SetItemAt(sourceIndex, sourceRemaining);
            }

            // Помещаем в назначение
            destinationInventory.SetItemAt(destinationIndex, new InventoryItem { item = itemToMove.item, quantity = quantityToMove });
        }
        // 2. Предметы одинаковые и можно стакать
        else if (InventoryItem.CanStackCheck(itemToMove, itemAtDestination))
        {
            int maxCanTake = itemAtDestination.MaxStackSize - itemAtDestination.quantity;
            int actualMoveAmount = Mathf.Min(quantityToMove, maxCanTake); // Сколько реально можем переместить

            if (actualMoveAmount > 0)
            {
                // Уменьшаем количество в источнике
                InventoryItem sourceRemaining = itemToMove.ChangeQuantity(itemToMove.quantity - actualMoveAmount);
                if (sourceRemaining.quantity <= 0)
                {
                    sourceInventory.SetItemAt(sourceIndex, InventoryItem.GetEmptyItem());
                }
                else
                {
                    sourceInventory.SetItemAt(sourceIndex, sourceRemaining);
                }

                // Увеличиваем количество в назначении
                destinationInventory.SetItemAt(destinationIndex, itemAtDestination.ChangeQuantity(itemAtDestination.quantity + actualMoveAmount));
            }
            else // Если стакать некуда (dest полный), и это был полный драг ЛКМ - меняем местами
            {
                if (quantityToMove == itemToMove.quantity) // Проверяем, был ли это полный драг
                {
                    sourceInventory.SetItemAt(sourceIndex, itemAtDestination);
                    destinationInventory.SetItemAt(destinationIndex, itemToMove);
                }
                // Иначе (ПКМ драг на полный слот того же типа) - ничего не делаем
            }
        }
        // 3. Предметы разные ИЛИ одинаковые, но стакать нельзя/некуда (и это был полный драг)
        else if (quantityToMove == itemToMove.quantity) // Только если перетаскивали весь стак
        {
            // Меняем местами
            sourceInventory.SetItemAt(sourceIndex, itemAtDestination);
            destinationInventory.SetItemAt(destinationIndex, itemToMove);
        }
        // 4. Если предметы разные и это был частичный драг (ПКМ) - не позволяем обмен.
        // else { Debug.Log("Cannot split stack onto a different item type between inventories."); }

        sourceInventory.InformAboutChange();
        destinationInventory.InformAboutChange();

        Debug.Log($"Transferred/Swapped item from {sourceInventory} (idx {sourceIndex}) to {destinationInventory} (idx {destinationIndex})");
    }

    public InventoryItem StackIfAvailable(InventoryItem sourceItem, InventoryItem destinationItem)
    {
        InventoryItem finalItem = InventoryItem.GetEmptyItem();
        
        bool canStack = sourceItem.item.itemData == destinationItem.item.itemData && destinationItem.CanStack;
        if (canStack)
        {
            /* логика стакинга */
            int maxAvailableTransfer;
            maxAvailableTransfer = destinationItem.MaxStackSize - destinationItem.quantity;
            if (sourceItem.quantity == maxAvailableTransfer)
            {
                //sourceInventory.SetItemAt(sourceIndex, itemAtDestination);
                int moveAmount = sourceItem.quantity + maxAvailableTransfer;
                finalItem = new InventoryItem
                {
                    quantity = moveAmount,
                    item = sourceItem.item,
                };
                Debug.Log($"Stacking to maxAmount {moveAmount}");
            }
            else if (sourceItem.quantity < maxAvailableTransfer)
            {
                int moveAmount = sourceItem.quantity;
                finalItem = new InventoryItem
                {
                    quantity = moveAmount,
                    item = sourceItem.item,
                };
                Debug.Log($"Stacking, deleting sourceItem completly {moveAmount}");
            }
        }
        
        return finalItem;
    }

    public Item SelectItem(Inventory sourceInv, int slotId)
    {
        selectedItem = sourceInv.GetItemAt(slotId).item;

        if (selectedItem != null) // If slot has item
        {
            if (selectedWorldItem != null)
            { // это конечно сильно так каждый раз удалять и спавнить...
                Destroy(selectedWorldItem.gameObject);
            }

            selectedWorldItem = selectedItem.SpawnItem(rightHand.forward, 1);
            selectedWorldItem.Attach(rightHand);

            OnItemSelected?.Invoke(this, new OnItemSelectedEventArgs { selectedItem = selectedItem, worldItem = selectedWorldItem, slotId = slotId });

            //if (selectedItem != null)
            //    Debug.Log($"[{slotId}] Selected {selectedItem.itemName} item");
        }
        else // Switch to empty slot
        {
            if (selectedWorldItem != null)
                Destroy(selectedWorldItem.gameObject);

            OnItemSelected?.Invoke(this, new OnItemSelectedEventArgs { selectedItem = selectedItem, worldItem = null, slotId = slotId });
        }

        return selectedItem;
    }
    
    /// <summary>
    /// Global search in all player's inventories
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Inventory the item was found in and Item index</returns>
    public (Inventory foundInventory, int index) HasItem(Item item)
    {
        int inventoryIndex = inventory.HasItem(item);
        int hotbarIndex = hotbar.HasItem(item);
        
        if (inventoryIndex != -1){
            return (inventory, inventoryIndex);
        }
        if(hotbarIndex != -1){
            return (hotbar, inventoryIndex);;
        }
        
        return (null, -1);
    }
    /// <summary>
    /// Global search in all player's inventories
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Inventory the item was found in and Item index</returns>
    public (Inventory foundInventory, int index) HasItem(ItemData item)
    {
        int inventoryIndex = inventory.HasItem(item);
        int hotbarIndex = hotbar.HasItem(item);
        
        if (inventoryIndex != -1){
            return (inventory, inventoryIndex);
        }
        if(hotbarIndex != -1){
            return (hotbar, hotbarIndex);
        }
        
        return (null, -1);
    }
    public int ItemAmount(ItemData itemData)
    {
        int amount = 0;
        amount += inventory.ItemAmount(itemData) + hotbar.ItemAmount(itemData);
        return amount;
    }
    
    public bool CheckItemRequirements(ItemRequirements requirements) // сделать на уровне инвентаря
    {   
        foreach(ItemRequirement req in requirements.requirements)
        {
            (Inventory inv, int index) = HasItem(req.item);
            if(index == -1 || ItemAmount(req.item) < req.quantity)
            {
                return false;
            }
        }
        
        return true;
    }
    public bool CheckItemRequirements(ItemRequirements requirements, Inventory inventory) // сделать на уровне инвентаря
    {   
        foreach(ItemRequirement req in requirements.requirements)
        {
            int index = inventory.HasItem(req.item);
            if(index == -1 || inventory.ItemAmount(req.item) < req.quantity)
            {
                return false;
            }
        }
        
        return true;
    }
    public void ConsumeItemRequirements(ItemRequirements requirements)
    {
        // ВПРИНЦИПЕ проблему с несколькими instancами нужного предмета можно пофиксить
        // записывать текущее удалённое количество и если оно меньше чем надо то искать ещё раз
        foreach(ItemRequirement req in requirements.requirements)
        {
            DeleteAmount(req.item, req.quantity);
            //(Inventory inv, int index) = HasItem(req.item);
            //inv.DeleteItem(index, req.quantity);
        }
    }
    public int[] GetItemAmounts(ItemRequirement[] items)
    {
        int[] finalList = new int[items.Length];
        for(int i = 0; i < items.Length; i++)
        {
            finalList[i] = ItemAmount(items[i].item);
        }
        
        return finalList;
    }
    public int[] GetItemAmounts(ItemData[] items)
    {
        int[] finalList = new int[items.Length];
        for(int i = 0; i < items.Length; i++)
        {
            finalList[i] = ItemAmount(items[i]);
        }
        
        return finalList;
    }
    
    public bool TryAddItem(Item itemToAdd, int quantity)
    {
        if(hotbar.AddItem(itemToAdd, quantity)){
            TextMeshProUGUI popUp = WorldUI.i.SpawnPopup(
                transform.position+new Vector3(0, 1f, 0),
                Color.white, 
                new Vector3(1f, 1f, 1f),
                0.75f,
                3f);
            popUp.fontSize = 8;
            popUp.text = $"+{quantity} {itemToAdd.itemName}";
            return true;
        }
        if(inventory.AddItem(itemToAdd, quantity)){
            TextMeshProUGUI popUp = WorldUI.i.SpawnPopup(
                transform.position+new Vector3(0, 1f, 0),
                Color.white, 
                new Vector3(1.05f, 1.05f, 1.05f),
                0.75f,
                3f);
            popUp.fontSize = 8;
            popUp.text = $"+{quantity} {itemToAdd.itemName}";
            return true;
        }
        
        return false;
    }
    
    public bool DeleteAmount(ItemData item, int quantity)
    {
        if(ItemAmount(item) < quantity){
            Debug.LogWarning("Not enough items to delete.");
            return false;
        }
        
        int maxIterations = 15; // защита
        //int itemsDeleted = 0;
        int quantityTarget = ItemAmount(item) - quantity;
        
        int itemsToDelete = quantity;
        // while(itemsDeleted != quantity && maxIterations > 0)
        // {
                //int itemsDeleted = 0;
        
        //     (Inventory inv, int index) = HasItem(item);
        //     itemsDeleted += inv.DeleteAmount(index, itemsToDelete);
        //     itemsToDelete -= itemsDeleted;
            
        //     Debug.Log($"[{item.itemName}] left to delete: " + itemsToDelete);
            
        //     maxIterations--;
        // }
        
        while(ItemAmount(item) != quantityTarget && maxIterations > 0)
        {
            int itemsDeleted = 0;
            
            (Inventory inv, int index) = HasItem(item);
            itemsDeleted += inv.DeleteAmount(index, itemsToDelete);
            itemsToDelete -= itemsDeleted;
            
            //Debug.Log($"[{item.itemName}] left to delete: " + itemsToDelete);
            
            maxIterations--;
        }
        
        return true;
    }
    
    private void UpdateSelectedItem(Dictionary<int, InventoryItem> inventoryState) // hotbar only for now
    {
        if (!hotbar.HasItemAt(selectedSlotId)) // если предмет пропал из выбранного слота
        {
            SelectItem(hotbar, selectedSlotId);
            // if (selectedWorldItem != null)
            //     Destroy(selectedWorldItem.gameObject);
            // selectedItem = null;

            // OnItemSelected?.Invoke(this, new OnItemSelectedEventArgs { selectedItem = selectedItem, worldItem = null, slotId = selectedSlotId });
        }
        else // если предмет появился в выбранном слоте
        {
            SelectItem(hotbar, selectedSlotId);
        }
    }

    void Update()
    {
        // if (controls.Player.Next.WasPressedThisFrame() && selectedSlotId < hotbar.Size - 1)
        // {
        //     selectedSlotId++;
        //     SelectItem(hotbar, selectedSlotId);
        // }
        // else if (controls.Player.Previous.WasPressedThisFrame() && selectedSlotId > 0)
        // {
        //     selectedSlotId--;
        //     SelectItem(hotbar, selectedSlotId);
        // }
        
        // TO-DO: придумать систему лучше этого бреда + сделать визуализацию выбора
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            //selectedSlotId++;
            selectedSlotId = 0;
            SelectItem(hotbar, selectedSlotId);
            hotbarUI.HandleItemSelection(selectedSlotId);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSlotId = 1;
            SelectItem(hotbar, selectedSlotId);
            hotbarUI.HandleItemSelection(selectedSlotId);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSlotId = 2;
            SelectItem(hotbar, selectedSlotId);
            hotbarUI.HandleItemSelection(selectedSlotId);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedSlotId = 3;
            SelectItem(hotbar, selectedSlotId);
            hotbarUI.HandleItemSelection(selectedSlotId);
        }
        
        HandlePickup();

        if (controls.Player.Drop.WasPressedThisFrame() && selectedItem != null) // тоже доработать под несколько инвентарей
        {
            Item droppedItem = hotbar.DropItem(selectedSlotId, dropPoint.position, dropPoint.forward);
            if (selectedItem == droppedItem && !hotbar.HasItemAt(selectedSlotId))
            {
                selectedItem = null;
                Destroy(selectedWorldItem.gameObject);
            }
            OnItemDropped?.Invoke(this, new OnItemDroppedEventArgs { droppedItem = droppedItem, slotId = selectedSlotId });
        }

        if (controls.Player.InventoryOpen.WasPressedThisFrame())
        {
            if (!inventoryUI.isActiveAndEnabled)
            {
                inventoryUI.Show();
                foreach (var item in inventory.GetCurrentInventoryState())
                {
                    inventoryUI.UpdateData(item.Key, item.Value.item.icon, item.Value.quantity, item.Value);
                }
            }
            else
            {
                inventoryUI.Hide();
            }
        }
        
    }

    public WorldItem nearbyItem;
    public void HandlePickup()
    {
        if(nearbyItem == null) return;
        
        if(controls.Player.Interact.WasPressedThisFrame())
        {
            if(TryAddItem(nearbyItem.originItem, nearbyItem.quantity)){
                Destroy(nearbyItem.gameObject);
                
                CheckForNearbyItems(); // не работает
            }
        }
    }
    public bool CheckForNearbyItems()
    {
        Collider[] hitColliders = new Collider[1];
        Physics.OverlapSphereNonAlloc(transform.position, 1.5f, hitColliders, LayerMask.GetMask("Items"));
        if(hitColliders.Length > 0)
        {
            nearbyItem = hitColliders[0].GetComponent<WorldItem>();
            return true;
        }
        
        nearbyItem = null;
        return false;
    }
    
    public void OnTriggerEnter(Collider col)
    {
        WorldItem item = null;
        col.TryGetComponent(out item);
        if(item != null && item.dropped)
        {
            if(item.itemData.instantPickup)
            {
                if(TryAddItem(item.originItem, item.quantity)){
                    Destroy(col.gameObject);
                }
            }
            else{
                nearbyItem = item;
                // ui & button interaction
            }
        }
    }
    public void OnTriggerExit(Collider col)
    {
        nearbyItem = null;
    }

    public void OnInventoryUpdated(Dictionary<int, InventoryItem> invState)
    {
        OnInventoriesUpdated?.Invoke();
    }
    public event Action OnInventoriesUpdated;
    
    public event EventHandler<OnItemSelectedEventArgs> OnItemSelected;
    public class OnItemSelectedEventArgs : EventArgs
    {
        public Item selectedItem;
        public WorldItem worldItem;
        public int slotId;
    }

    public event EventHandler<OnItemPickedUpEventArgs> OnItemPickedUp;
    public class OnItemPickedUpEventArgs : EventArgs
    {
        public Item pickedUpItem;
        public int placedSlotId;
    }
    
    public event EventHandler<OnItemDroppedEventArgs> OnItemDropped;
    public class OnItemDroppedEventArgs : EventArgs
    {
        public Item droppedItem;
        public int slotId;
    }
}
