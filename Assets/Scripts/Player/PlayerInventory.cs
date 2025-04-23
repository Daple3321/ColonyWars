using System;
using System.Collections.Generic;
using System.Linq;
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

    private Player player;
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
        

        PrepareUI();

        inventory.AddItem(new RangedWeapon(itemDatas[1]), 1);
        inventory.AddItem(new MeleeWeapon(itemDatas[0]), 1);
        //inventory.AddItem(new Item(itemDatas[2]), 5);
        //inventory.AddItem(new Item(itemDatas[2]), 8);
        inventory.AddItem(new RangedWeapon(itemDatas[3]), 1);
        inventory.AddItem(new RangedWeapon(itemDatas[4]), 1);
        
        
        hotbar.AddItem(new RangedWeapon(itemDatas[1]), 1);
        hotbar.AddItem(new MeleeWeapon(itemDatas[0]), 1);
        hotbar.AddItem(new Item(itemDatas[2]), 3);
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
        this.inventoryUI.OnItemActionRequested += HandleItemActionRequest;
        this.inventoryUI.OnTransferItemsRequest += HandleTransferRequest;
        inventoryUI.SetLinkedInventory(inventory);

        GameObject hotbarObj = Instantiate(GameAssets.hotbarUI_Prefab, GameController.i.mainCanvas.transform);
        hotbarUI = hotbarObj.GetComponent<InventoryUI>();
        hotbarUI.InitializeInventoryUI(hotbarStartingSize);
        this.hotbarUI.OnSwapItems += HandleSwapItemsHotbar;
        this.hotbarUI.OnStartDragging += HandleDraggingHotbar;
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

    private void HandleDragging(int itemIndex)
    {
        HandleDraggingInternal(inventory, inventoryUI, itemIndex);
    }
    private void HandleDraggingHotbar(int itemIndex)
    {
        HandleDraggingInternal(hotbar, hotbarUI, itemIndex);
    }

    private void HandleDraggingInternal(Inventory sourceInv, InventoryUI sourceUI, int itemIndex)
    {
        InventoryItem inventoryItem = sourceInv.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;
        sourceUI.CreateDraggedItem(inventoryItem.item.icon, inventoryItem.quantity, inventoryItem);
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

    private void HandleSwapInternal(Inventory sourceInv, int itemIndex_1, int itemIndex_2, InventoryItem from, InventoryItem to)
    {
        sourceInv.SwapItems(itemIndex_1, itemIndex_2, from, to);
    }
    
    private void HandleTransferRequest(InventoryUI sourceUI, int sourceIndex, InventoryUI destinationUI, int destinationIndex)
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

        // --- Логика переноса/обмена между инвентарями ---

        // 1. Простой случай: перемещение в пустой слот назначения
        if (itemAtDestination.IsEmpty)
        {
            sourceInventory.SetItemAt(sourceIndex, InventoryItem.GetEmptyItem()); // Очищаем источник
            destinationInventory.SetItemAt(destinationIndex, itemToMove);       // Помещаем в назначение
        }
        // 2. Сложный случай: обмен предметами между слотами разных инвентарей
        else
        {
            bool canStack = InventoryItem.CanStackCheck(itemToMove, itemAtDestination);
            if (canStack)
            {
                /* логика стакинга */
                // int maxAvailableTransfer;
                // maxAvailableTransfer = itemAtDestination.MaxStackSize - itemAtDestination.quantity;
                // if (itemToMove.quantity == maxAvailableTransfer)
                // {
                //     //sourceInventory.SetItemAt(sourceIndex, itemAtDestination);
                //     int moveAmount = itemToMove.quantity + maxAvailableTransfer;
                //     sourceInventory.DestroyItem(itemToMove);
                //     destinationInventory.SetItemAt(destinationIndex, new InventoryItem
                //     {
                //         quantity = moveAmount,
                //         item = itemToMove.item,
                //     });
                //     Debug.Log($"Stacking to maxAmount {moveAmount}");
                // }
                // else if (itemToMove.quantity < maxAvailableTransfer)
                // {
                //     int moveAmount = itemToMove.quantity + itemAtDestination.quantity;
                //     sourceInventory.DestroyItem(itemToMove);
                //     destinationInventory.SetItemAt(destinationIndex, new InventoryItem
                //     {
                //         quantity = moveAmount,
                //         item = itemToMove.item,
                //     });
                //     Debug.Log($"Stacking, deleting sourceItem completly {moveAmount}");
                // }
                InventoryItem sourceItemChanged;
                InventoryItem finalItem = InventoryItem.Stack(itemToMove, itemAtDestination, out sourceItemChanged);
                sourceInventory.SetItemAt(sourceIndex, sourceItemChanged);
                destinationInventory.SetItemAt(destinationIndex, finalItem);
            }
            else
            {
                /* логика обмена */
                Debug.Log("Cant stack, swapping.");
                sourceInventory.SetItemAt(sourceIndex, itemAtDestination);
                destinationInventory.SetItemAt(destinationIndex, itemToMove);
            }
        }
        
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

            selectedWorldItem = selectedItem.SpawnItem(rightHand);
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
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSlotId = 1;
            SelectItem(hotbar, selectedSlotId);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSlotId = 2;
            SelectItem(hotbar, selectedSlotId);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedSlotId = 3;
            SelectItem(hotbar, selectedSlotId);
        }

        if (controls.Player.Drop.WasPressedThisFrame() && selectedItem != null) // тоже доработать под несколько инвентарей
        {
            Item droppedItem = hotbar.DropItem(selectedSlotId, transform);
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
