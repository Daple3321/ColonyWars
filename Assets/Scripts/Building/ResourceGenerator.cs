using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceGenerator : Generator
{
    protected ResourceGeneratorData generatorData;
    
    public float gatherRate = 5f;
    private float _gatherRate = 5f;
    public int yieldAmount = 1;
    
    public List<ResourceNode> resourcesNearby;
    
    public Inventory inventory;
    public int inventoryStartingSize = 2;
    public InventoryUI inventoryUI;
    
    private PlayerInventory playerInventory;
    
    public override void Init(BuildingData data)
    {
        meshes = transform.GetComponentsInChildren<MeshRenderer>();
        resourcesNearby = new List<ResourceNode>();
        _gatherRate = gatherRate;
        this.buildingData = data;
        
        playerInventory = GameController.p.playerInventory;
        
        if(data is ResourceGeneratorData resourceGeneratorData){
            this.generatorData = resourceGeneratorData;
        }
        
        inventory = new Inventory(inventoryStartingSize);
        inventory.OnInventoryUpdated += UpdateUI;
        
        //PrepareUI();
    }
    
    // ЭТО ВСЁ МОЖНО ПЕРЕНЕСТИ В BuildingInventory.cs класс какой-нить.
    public override void PrepareUI()
    {
        GameObject invObj = Instantiate(GameAssets.hotbarUI_Prefab);
        invObj.transform.SetParent(GameController.i.buildingPanelManager.currentPanel.transform);
        
        inventoryUI = invObj.GetComponent<InventoryUI>();
        inventoryUI.InitializeInventoryUI(inventoryStartingSize);
        this.inventoryUI.OnSwapItems += HandleSwapItems;
        this.inventoryUI.OnStartDragging += HandleDragging;
        this.inventoryUI.OnItemVoidDrop += HandleVoidDrop;
        this.inventoryUI.OnTransferItemsRequest += playerInventory.HandleTransferRequest;
        inventoryUI.SetLinkedInventory(inventory);
        
        //UpdateUI(inventory.GetCurrentInventoryState());
    }
    public override void ClearUI()
    {
        this.inventoryUI.OnSwapItems -= HandleSwapItems;
        this.inventoryUI.OnStartDragging -= HandleDragging;
        this.inventoryUI.OnItemVoidDrop -= HandleVoidDrop;
        this.inventoryUI.OnTransferItemsRequest -= playerInventory.HandleTransferRequest;
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
            inventory.DropWholeStack(itemIndex, transform);
        }
        else{
            inventory.DropItem(itemIndex, transform, quantity);
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
    
    
    public override IEnumerator Build()
    {
        float timeLeft = 0;
        while (timeLeft < buildTime)
        {
            buildProgress = timeLeft/buildTime;
            
            ChangeColor(Color.Lerp(Color.black, Color.white, buildProgress));
            timeLeft += Time.deltaTime;
            yield return null;
        }
        
        built = true;
        buildProgress = 1f;
        ChangeColor(Color.white);
        
        resourcesNearby = CheckForResources().ToList();
        foreach(ResourceNode node in resourcesNearby){
            node.OnResourceDeleted += OnResourceDeleted;
        }
    }

    void Update()
    {
        if(resourcesNearby != null && inventory.SpaceLeft() > 0){
            HandleGathering();
        }
    }
    
    public void HandleGathering(){
        if(_gatherRate > 0){
            _gatherRate -= Time.deltaTime;
        }
        else{
            Gather();
            _gatherRate = gatherRate;
        }
    }

    public virtual void Gather()
    {
        foreach(ResourceNode node in resourcesNearby) // ошибка при удалении нода
        {
            inventory.AddItem(node.GeneratorGather(), yieldAmount);
            
            Debug.Log("Space left: " + inventory.SpaceLeft());
            // add to inventory
        }
    }
    
    public ResourceNode[] CheckForResources()
    {
        Collider[] resources = Physics.OverlapSphere(transform.position, generatorData.gatherRadius, generatorData.resourceMask);
        if(resources.Length > 0)
        {
            // это вообще уже НЕ НОРМАЛЬНО.
            Collider[] neededNodes = resources.ToList().FindAll(
                x => generatorData.resourceConditions.Contains(x.GetComponent<ResourceNode>().resource)
            ).ToArray();
            
            if(neededNodes.Length > 0)
            {
                ResourceNode[] nodes = new ResourceNode[neededNodes.Length];
                for(int i = 0; i < neededNodes.Length; i++){
                    nodes[i] = neededNodes[i].GetComponent<ResourceNode>();
                }
                return nodes;
            }
        }
        return null;
    }
    
    public void OnResourceDeleted(ResourceNode node)
    {
        resourcesNearby.Remove(node);
    }
}
