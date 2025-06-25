using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInventory
{
    public Inventory inventory;
    public int inventoryStartingSize = 2;
    public InventoryUI inventoryUI;
    
    private PlayerInventory playerInventory;
    
    private Building buildingOwner;
    public BuildingInventory(Building building, int startingSize = 2)
    {
        this.buildingOwner = building;
        
        playerInventory = GameController.p.playerInventory;
        inventoryStartingSize = startingSize;
        
        inventory = new Inventory(inventoryStartingSize);
        inventory.OnInventoryUpdated += UpdateUI;
    }
    
    public void PrepareUI(bool transferPriority = false, InventoryUI transferInventory = null, RectTransform invParent = null, string invName = "")
    {
        GameObject invObj = GameObject.Instantiate(GameAssets.buildingInventory);
        TextMeshProUGUI invText = invObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        if(invName != ""){
            invText.text = invName;
        }
        else{
            invText.gameObject.SetActive(false);
        }
        
        if(invParent == null){
            invObj.transform.SetParent(GameController.i.buildingPanelManager.currentPanel.dataContainter);
        }
        else{
            invObj.transform.SetParent(invParent);
        }
        invObj.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        
        inventoryUI = invObj.GetComponent<InventoryUI>();
        inventoryUI.InitializeInventoryUI(inventoryStartingSize);
        this.inventoryUI.OnSwapItems += HandleSwapItems;
        this.inventoryUI.OnStartDragging += HandleDragging;
        this.inventoryUI.OnItemVoidDrop += HandleVoidDrop;
        this.inventoryUI.OnTransferItemsRequest += playerInventory.HandleTransferRequest;
        this.inventoryUI.OnFastTransferRequest += playerInventory.HandleFastTranferRequest;
        inventoryUI.SetLinkedInventory(inventory);
        inventoryUI.SetTransferInventory(transferInventory);
        
        this.inventoryUI.Show(transferPriority);
        //UpdateUI(inventory.GetCurrentInventoryState());
    }
    public void ClearUI()
    {
        this.inventoryUI.OnSwapItems -= HandleSwapItems;
        this.inventoryUI.OnStartDragging -= HandleDragging;
        this.inventoryUI.OnItemVoidDrop -= HandleVoidDrop;
        this.inventoryUI.OnTransferItemsRequest -= playerInventory.HandleTransferRequest;
        this.inventoryUI.OnFastTransferRequest -= playerInventory.HandleFastTranferRequest;
        
        this.inventoryUI.Hide();
    }
    
    private void HandleSwapItems(int itemIndex_1, int itemIndex_2, InventoryItem from, InventoryItem to)
    {
        inventory.SwapItems(itemIndex_1, itemIndex_2, from, to, DragDropManager.dragQuantity);
    }
    private void HandleDragging(int itemIndex, int dragQuantity)
    {
        InventoryItem inventoryItem = inventory.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;
        
        if(dragQuantity == -1){ // взять полностью весь стак
            inventoryUI.CreateDraggedItem(inventoryItem.item.icon, inventoryItem.quantity, inventoryItem);
        }
        else{
            inventoryUI.CreateDraggedItem(inventoryItem.item.icon, dragQuantity, inventoryItem);
        }
    }
    private void HandleVoidDrop(int itemIndex, int quantity)
    {
        if(quantity == -1){
            inventory.DropWholeStack(itemIndex, buildingOwner.transform.position+new Vector3(0, 2f, 0), Vector3.up, 360);
        }
        else{
            inventory.DropItem(itemIndex, buildingOwner.transform.position+new Vector3(0, 2f, 0), Vector3.up, 360, quantity);
        }
    }
    public void UpdateUI(Dictionary<int, InventoryItem> inventoryState)
    {
        if(inventoryUI != null)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.icon, item.Value.quantity, item.Value);
            }
        }
    }
    
    public void DropAllItems()
    {
        Dictionary<int, InventoryItem> inventoryState = inventory.GetCurrentInventoryState();
        
        foreach(var item in inventoryState)
        {
            inventory.DropWholeStack(item.Key, buildingOwner.transform.position+new Vector3(0, 2f, 0), Vector3.up, 360);
        }
    }
}
