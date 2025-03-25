using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory inventory;
    
    public List<ItemData> itemDatas = new List<ItemData>();

    public GameObject itemPrefab;

    public int selectedSlotId = 0;
    public Item selectedItem = null;
    

    private Controls controls;
    void Awake()
    {
        enabled = false;
    }

    private Player player;
    public void Init(Player player)
    {
        this.player = player;
        controls = GameAssets.controls;
        enabled = true;

        inventory = new Inventory(8);

        inventory.AddItem(new RangedWeapon(itemDatas[1]), 1);
        inventory.AddItem(new MeleeWeapon(itemDatas[0]), 1);
        inventory.AddItem(new Item(itemDatas[2]), 5);

        //inventory.DeleteItem(2);
        inventory.DropItem(2, transform, 1);
        
        inventory.PrintInv();

        //GameObject droppedItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        //WorldItem worldItem = droppedItem.GetComponent<WorldItem>();
        //worldItem.Initialize(inventory.GetItem(0).itemData, inventory.GetItem(0));

        SelectItem(selectedSlotId);
    }

    public Item SelectItem(int slotId)
    {
        selectedItem = inventory.GetItem(slotId);
        OnItemSelected?.Invoke(this, new OnItemSelectedEventArgs { selectedItem = selectedItem, slotId = slotId });
        if(selectedItem != null)
            Debug.Log($"[{slotId}] Selected {selectedItem.itemName} item");
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
            if (selectedItem == droppedItem)
            {
                selectedItem = null; // если выбрасываешь стакаемый предмет это обнуляется (хотя предмет ещё есть)
            }
            OnItemDropped?.Invoke(this, new OnItemDroppedEventArgs { droppedItem = droppedItem, slotId = selectedSlotId });
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            inventory.PrintInv();
        }
    }



    public event EventHandler<OnItemSelectedEventArgs> OnItemSelected;
    public class OnItemSelectedEventArgs : EventArgs
    {
        public Item selectedItem;
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
