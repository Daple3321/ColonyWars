using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private GameObject slotPrefab;

    [SerializeField]
    private RectTransform contentPanel;

    public List<InventorySlot> inventorySlots = new List<InventorySlot>();


    [SerializeField] private MouseFollower mouseFollower;

    private int currentlyDraggedItemIndex = -1;

    public event Action<int> OnItemActionRequested, OnStartDragging;
    public event Action<int, int> OnSwapItems;

    void Awake()
    {
        
    }

    public void InitializeInventoryUI(int size)
    {
        for (int i = 0; i < size; i++)
        {
            InventorySlot slot = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity).GetComponent<InventorySlot>();
            slot.transform.SetParent(contentPanel);
            inventorySlots.Add(slot);

            slot.OnItemClicked += HandleItemSelection;
            slot.OnItemBeginDrag += HandleBeginDrag;
            slot.OnItemDropped += HandleSwap;
            slot.OnItemEndDrag += HandleEndDrag;
            slot.OnRightClick += HandleShowItemActions;
        }

        Hide();
        mouseFollower.Toggle(false);
    }

    public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
    {
        if (inventorySlots.Count > itemIndex)
        {
            inventorySlots[itemIndex].SetData(itemImage, itemQuantity);
        }
    }

    private void HandleShowItemActions(InventorySlot slot)
    {

    }

    public void CreateDraggedItem(Sprite sprite, int quantity)
    {
        mouseFollower.Toggle(true);
        mouseFollower.SetData(sprite, quantity);
    }
    
    private void HandleBeginDrag(InventorySlot slot)
    {
        int index = inventorySlots.IndexOf(slot);
        if (index == -1)
            return;
        currentlyDraggedItemIndex = index;
        HandleItemSelection(slot);
        OnStartDragging?.Invoke(index);
    }
    private void HandleEndDrag(InventorySlot slot)
    {
        ResetDraggedItem();
    }

    private void ResetDraggedItem()
    {
        mouseFollower.Toggle(false);
        currentlyDraggedItemIndex = -1;
    }

    private void HandleSwap(InventorySlot slot)
    {
        int index = inventorySlots.IndexOf(slot);
        if (index == -1)
        {
            return;
        }
        OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
        HandleItemSelection(slot);
    }


    private void HandleItemSelection(InventorySlot slot)
    {
        int index = inventorySlots.IndexOf(slot);
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
