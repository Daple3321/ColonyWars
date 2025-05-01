using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private GameObject slotPrefab;

    [SerializeField]
    private RectTransform contentPanel;

    public List<InventorySlot> inventorySlots = new List<InventorySlot>();

    public bool hideOnInit = true;


    [SerializeField] private MouseFollower mouseFollower;

    private int currentlyDraggedItemIndex = -1;

    public event Action<int> OnItemActionRequested;
    public event Action<int, int, InventoryItem, InventoryItem> OnSwapItems;
    public event Action<InventoryUI, int, InventoryUI, int> OnTransferItemsRequest;
    public event Action<int, int> OnItemVoidDrop;
    public event Action<int, int> OnStartDragging;

    public Inventory LinkedInventory { get; private set; }
    public void SetLinkedInventory(Inventory linkedInventory)
    {
        LinkedInventory = linkedInventory;
    }
    
    public void InitializeInventoryUI(int size)
    {
        mouseFollower = GameController.i.mouseFollower;

        for (int i = 0; i < size; i++)
        {
            InventorySlot slot = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity).GetComponent<InventorySlot>();
            slot.transform.SetParent(contentPanel);
            slot.Init(this);
            inventorySlots.Add(slot);

            slot.OnItemClicked += HandleItemSelection;
            slot.OnItemBeginDrag += HandleBeginDrag;
            slot.OnItemDropped += HandleDrop;
            slot.OnItemVoidDrop += HandleVoidDrop;
            slot.OnItemEndDrag += HandleEndDrag;
            slot.OnRightClick += HandleShowItemActions;
        }

        if (hideOnInit)
        {
            Hide();
            mouseFollower.Toggle(false);
        }
    }

    public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity, InventoryItem item)
    {
        if (inventorySlots.Count > itemIndex)
        {
            inventorySlots[itemIndex].SetData(itemImage, itemQuantity, item);
        }
    }

    private void HandleShowItemActions(InventorySlot slot)
    {
        
    }

    public void CreateDraggedItem(Sprite sprite, int quantity, InventoryItem item)
    {
        mouseFollower.Toggle(true);
        mouseFollower.SetData(sprite, quantity, item);
    }
    
    private void HandleBeginDrag(InventorySlot slot, bool isRightClickDrag)
    {
        int index = inventorySlots.IndexOf(slot);
        if (index == -1 || slot.empty)
            return;
        
        //Debug.Log($"[HandleBeginDrag], begin drag. Slot is not empty and index != -1");
        currentlyDraggedItemIndex = index;
        DragDropManager.currentlyDraggedSlot = slot;
        if(isRightClickDrag){
            DragDropManager.dragQuantity = slot.item.HalfQuantity();
        }
        else{
            DragDropManager.dragQuantity = slot.item.quantity;
        }
        HandleItemSelection(slot);
        OnStartDragging?.Invoke(index, DragDropManager.dragQuantity); // создаёт mouseFollower
    }
    private void HandleEndDrag(InventorySlot slot)
    {
        ResetDraggedItem();
    }

    private void ResetDraggedItem()
    {
        mouseFollower.Toggle(false);
        currentlyDraggedItemIndex = -1;
        DragDropManager.currentlyDraggedSlot = null;
        DragDropManager.dragQuantity = -1;
    }
    
    private void HandleVoidDrop(InventorySlot dropFromSlot, int quantity)
    {
        int slotIndex = inventorySlots.IndexOf(dropFromSlot);
        if(slotIndex == -1){
            Debug.LogWarning($"No slot found while dropping in void");
            return;
        }
        
        OnItemVoidDrop?.Invoke(slotIndex, quantity);
    }

    private void HandleDrop(InventorySlot destinationSlot, int quantity)
    {
        if (DragDropManager.currentlyDraggedSlot == null) return;

        int destinationIndex = inventorySlots.IndexOf(destinationSlot);

        InventoryUI sourceUI = DragDropManager.currentlyDraggedSlot.ParentUI;
        int sourceIndex = sourceUI.inventorySlots.IndexOf(DragDropManager.currentlyDraggedSlot);
        InventoryItem sourceItem = DragDropManager.currentlyDraggedSlot.item;
        
        //Debug.Log($"[HandleDrop] destIndex = {destinationIndex}");
        if (sourceUI == this) // если обмен внутри одного инвентаря
        {
            if (destinationIndex == -1) // Упали на свой UI, но не на слот? (Можно обработать или игнорировать)
            {
                Debug.Log("Dropped onto self UI but not a specific slot.");
                return;
            }
            Debug.Log($"Swapping within {this.name}: Slot {sourceIndex} <-> Slot {destinationIndex}");
            // Используем существующее событие для обмена внутри одного инвентаря
            OnSwapItems?.Invoke(sourceIndex, destinationIndex, sourceItem, destinationSlot.item);
            HandleItemSelection(destinationSlot); // Выделяем целевой слот
        }
        else // перемещение между инвентарями
        {
            if (destinationIndex == -1)
            {
                Debug.LogError("Destination slot index not found in target UI. This shouldn't happen if dropped on a valid slot.");
                return;
            }

            Debug.Log($"Transfer Request: From '{sourceUI.name}' (Slot {sourceIndex}) To '{this.name}' (Slot {destinationIndex})");

            // Нужно новое событие для передачи данных о переносе в PlayerInventory или другой менеджер
            // Передаем: исходный UI, исходный индекс, целевой UI, целевой индекс
            OnTransferItemsRequest?.Invoke(sourceUI, sourceIndex, this, destinationIndex);
        }
    }


    public void HandleItemSelection(InventorySlot slot)
    {
        int index = inventorySlots.IndexOf(slot);
        if (index == -1)
            return;
        DeselectAllItems();
        inventorySlots[index].Select();
        // descriptionRequested
    }
    public void HandleItemSelection(int index)
    {
        //int index = inventorySlots.IndexOf(slot);
        if (index == -1)
            return;
        DeselectAllItems();
        inventorySlots[index].Select();
        // descriptionRequested
    }

    public void Show()
    {
        gameObject.SetActive(true);
        ResetSelection();
    }

    public void ResetSelection()
    {
        DeselectAllItems();
    }

    public void DeselectAllItems()
    {
        foreach (InventorySlot item in inventorySlots)
        {
            item.Deselect();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        ResetDraggedItem();
    }

    public void ResetAllItems()
    {
        foreach (var item in inventorySlots)
        {
            item.ResetData();
            item.Deselect();
        }
    }
}
