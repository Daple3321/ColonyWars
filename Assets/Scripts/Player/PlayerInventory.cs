using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory inventory;
    public int inventoryStartingSize = 8;
    
    public List<ItemData> itemDatas = new List<ItemData>();

    public GameObject itemPrefab;

    public int selectedSlotId = 0;
    public Item selectedItem = null;
    public WorldItem selectedWorldItem;

    private Controls controls;
    void Awake()
    {
        enabled = false;
    }

    private Player player;
    private Transform rightHand;
    private Transform leftHand;
    [SerializeField] private InventoryUI inventoryUI;
    public void Init(Player player)
    {
        this.player = player;
        rightHand = player.rightHand;
        leftHand = player.leftHand;
        controls = GameAssets.controls;
        enabled = true;

        inventory = new Inventory(inventoryStartingSize);
        inventory.OnInventoryUpdated += UpdateUI;

        inventory.AddItem(new RangedWeapon(itemDatas[1]), 1);
        inventory.AddItem(new MeleeWeapon(itemDatas[0]), 1);
        inventory.AddItem(new Item(itemDatas[2]), 5);

        //inventory.DeleteItem(2);
        //inventory.DropItem(2, transform, 1);

        //inventoryUI = GameController.i.playerInventoryUI;
        PrepareUI();
        //inventoryUI.InitializeInventoryUI(inventoryStartingSize);
        //inventory.PrintInv();

        //GameObject droppedItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        //WorldItem worldItem = droppedItem.GetComponent<WorldItem>();
        //worldItem.Initialize(inventory.GetItem(0).itemData, inventory.GetItem(0));

        SelectItem(selectedSlotId);
    }

    private void UpdateUI(Dictionary<int, InventoryItem> inventoryState)
    {
        inventoryUI.ResetAllItems();
        foreach (var item in inventoryState)
        {
            inventoryUI.UpdateData(item.Key, item.Value.item.icon, item.Value.quantity);
        }
    }

    private void PrepareUI()
    {
        inventoryUI = GameController.i.playerInventoryUI;
        inventoryUI.InitializeInventoryUI(inventoryStartingSize);
        this.inventoryUI.OnSwapItems += HandleSwapItems;
        this.inventoryUI.OnStartDragging += HandleDragging;
        this.inventoryUI.OnItemActionRequested += HandleItemActionRequest;
    }

    private void HandleItemActionRequest(int itemIndex)
    {
        
    }

    private void HandleDragging(int itemIndex)
    {
        InventoryItem inventoryItem = inventory.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;
        inventoryUI.CreateDraggedItem(inventoryItem.item.icon, inventoryItem.quantity);
    }

    private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
    {
        inventory.SwapItems(itemIndex_1, itemIndex_2);
    }

    public Item SelectItem(int slotId)
    {
        selectedItem = inventory.GetItemAt(slotId).item;

        if (selectedItem != null) // If slot has item
        {
            if (selectedWorldItem != null)
            { // это конечно сильно так каждый раз удалять и спавнить...
                Destroy(selectedWorldItem.gameObject);
            }

            selectedWorldItem = selectedItem.SpawnItem(rightHand);
            selectedWorldItem.Attach(rightHand);

            OnItemSelected?.Invoke(this, new OnItemSelectedEventArgs { selectedItem = selectedItem, worldItem = selectedWorldItem, slotId = slotId });

            if (selectedItem != null)
                Debug.Log($"[{slotId}] Selected {selectedItem.itemName} item");
        }
        else // Switch to empty slot
        {
            if (selectedWorldItem != null)
                Destroy(selectedWorldItem.gameObject);

            OnItemSelected?.Invoke(this, new OnItemSelectedEventArgs { selectedItem = selectedItem, worldItem = null, slotId = slotId });
        }

        return selectedItem;
    }

    void Update()
    {
        if (controls.Player.Next.WasPressedThisFrame() && selectedSlotId < inventory.Size - 1)
        {
            selectedSlotId++;
            SelectItem(selectedSlotId);
        }
        else if (controls.Player.Previous.WasPressedThisFrame() && selectedSlotId > 0)
        {
            selectedSlotId--;
            SelectItem(selectedSlotId);
        }

        if (controls.Player.Drop.WasPressedThisFrame() && selectedItem != null)
        {
            Item droppedItem = inventory.DropItem(selectedSlotId, transform);
            if (selectedItem == droppedItem && !inventory.HasItemAt(selectedSlotId))
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
                    inventoryUI.UpdateData(item.Key, item.Value.item.icon, item.Value.quantity);
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
