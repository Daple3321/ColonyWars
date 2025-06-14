using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftingStation : Manufacturer
{
    public List<CraftRecipe> recipes;
    
    public bool craftInProgress;
    public float craftProgress;
    public CraftQueueElement currentCraft;
    public List<CraftQueueElement> queue;
    public int queueCapacity = 10;
    
    public BuildingInventory outputInv;
    public CraftingStationData stationData;
    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        outputInv = new BuildingInventory(this, 3);
        queue = new List<CraftQueueElement>();
        
        if(data is CraftingStationData csd){
            stationData = csd;
            recipes = csd.recipes;
        }
    }
    
    public override void PrepareUI()
    {
        originInv.PrepareUI(true, GameController.p.playerInventory.inventoryUI);
        originInv.UpdateUI(originInv.inventory.GetCurrentInventoryState());
        outputInv.PrepareUI(false, GameController.p.playerInventory.inventoryUI);
        outputInv.UpdateUI(outputInv.inventory.GetCurrentInventoryState());
        
        if(GameController.i.buildingPanelManager.currentPanel is CraftingStationPanel csp)
        {
            csp.InitCrafts(recipes.ToArray());
            csp.InitQueue(queue);
            // foreach(CraftSlot slot in csp.crafts){
            //     slot.OnCraftClicked += QueueCraft;
            // }
        }
    }
    public override void ClearUI()
    {
        originInv.ClearUI();
        outputInv.ClearUI();
        
        // if(GameController.i.buildingPanelManager.currentPanel is CraftingStationPanel csp)
        // {
        //     // foreach(CraftSlot slot in csp.crafts){
        //     //     slot.OnCraftClicked -= QueueCraft;
        //     // }
        // }
    }
    public override void OnDeath()
    {
        base.OnDeath();
        originInv.DropAllItems();
        outputInv.DropAllItems();
    }
    
    public override BuildingPanel CreatePanel(RectTransform parentContainer){
        GameObject go = Instantiate(GameAssets.craftingStationPanel, parentContainer);
        CraftingStationPanel panel = go.GetComponent<CraftingStationPanel>();
        //panel.Init(this);
        return panel;
    }

    void Update(){
        HandleCraftQueue();
    }

    public Action<float, float> OnCraftProgressChanged;
    public Action<List<CraftQueueElement>> OnQueueChanged;
    private void HandleCraftQueue()
    {
        if(!craftInProgress)
        {
            for (int i = 0; i < queue.Count; i++) // сильно конечно так лупится каждый фрейм
            {
                if (CanCraft(queue[i].recipe))
                {
                    currentCraft = queue[i];

                    if (currentCraft.IsLastCraft())
                    {
                        queue.RemoveAt(i);
                    }
                    else
                    {
                        currentCraft.Subtract();
                    }

                    craftInProgress = true;
                    craftProgress = 0;
                    OnCraftProgressChanged?.Invoke(craftProgress, currentCraft.recipe.craftTime);
                    OnQueueChanged?.Invoke(queue);
                    break; // нашли подходящий крафт — выходим из цикла
                }
            }
        }
        
        // КРАФТЫ КОТОРЫЕ НЕ МОЖЕТ СДЕЛАТЬ ВСЁ РАВНО ИДУТ И НЕ ТРАТЯТСЯ РЕСЫ
        // if(queue.Count > 0 && !craftInProgress && CanCraft(queue[0].recipe))
        // {
        //     currentCraft = queue[0];
        //     if(currentCraft.IsLastCraft()){
        //         queue.RemoveAt(0);
        //     }
        //     else{
        //         currentCraft.Subtract();
        //     }
        //     craftInProgress = true;
        //     craftProgress = 0;
        //     OnCraftProgressChanged?.Invoke(craftProgress, currentCraft.recipe.craftTime);
        //     OnQueueChanged?.Invoke(queue);
        // }
        
        if(craftInProgress){
            HandleCraftProgress();
        }
        // else if(craftInProgress && !CanCraft_Fast(currentCraft.recipe)){
        //     craftInProgress = false;
        //     craftProgress = 0;
        // }
    }
    
    private void HandleCraftProgress()
    {
        if(craftProgress < currentCraft.recipe.craftTime)
        {
            craftProgress += Time.deltaTime;
            OnCraftProgressChanged?.Invoke(craftProgress, currentCraft.recipe.craftTime);
        }
        else if(craftProgress >= currentCraft.recipe.craftTime)
        {
            Craft(currentCraft.recipe);
            craftInProgress = false;
            craftProgress = 0;
            OnCraftProgressChanged?.Invoke(craftProgress, 1);
        }
    }
    
    public void QueueCraft(CraftRecipe recipe)
    {
        if(queue.Count >= queueCapacity){return;}
        
        CraftQueueElement foundElement =  queue.Find(x => x.recipe == recipe);
        if(foundElement != null){
            //Debug.Log("Stacking craft!");
            foundElement.Add();
        }
        else{
            queue.Add(new CraftQueueElement(recipe));
        }
        
        OnQueueChanged?.Invoke(queue);
    }
    public void DequeueCraft(CraftRecipe recipe)
    {
        CraftQueueElement foundElement = queue.Find(x => x.recipe == recipe);
        if(foundElement != null){
            queue.Remove(foundElement);
            OnQueueChanged?.Invoke(queue);
        }
        else{
            Debug.LogError("Can't dequeue. Element with that craft not found!");
        }
    }
    
    public bool CanQueue(CraftRecipe recipe)
    {
        if(outputInv.inventory.EmptySlots() < 1
        && outputInv.inventory.HasItemWithSpaceLeft(recipe.finalItem, recipe.finalAmount) == -1){
            return false;
        }
        
        if(outputInv.inventory.EmptySlots() >= 1 ||
            outputInv.inventory.HasItemWithSpaceLeft(recipe.finalItem, recipe.finalAmount) != -1) 
        {
            return true;
        }
        
        return true;
    }
    
    public bool CanCraft(CraftRecipe recipe) // GC alloc every frame bruh this fucking LINQ
    {
        foreach (ItemRequirement req in recipe.requirements.requirements)
        {
            if (originInv.inventory.ItemAmount(req.item) < req.quantity)
                return false;
        }
        if(outputInv.inventory.EmptySlots() < 1
        && outputInv.inventory.HasItemWithSpaceLeft(recipe.finalItem, recipe.finalAmount) == -1){
            return false;
        }
        
        if(outputInv.inventory.EmptySlots() >= 1 ||
            outputInv.inventory.HasItemWithSpaceLeft(recipe.finalItem, recipe.finalAmount) != -1) 
        {
            return true;
        }
        
        return true;
    }

    public void Craft(CraftRecipe recipe)
    {
        if (!CanCraft(recipe)) return;

        originInv.inventory.ConsumeItemRequirements(recipe.requirements);

        outputInv.inventory.AddItem(recipe.finalItem.CreateItemInstance(), recipe.finalAmount);

        return;
    }
}

[System.Serializable]
public class CraftQueueElement
{
    public CraftRecipe recipe;
    public int amountToCraft;
    
    public CraftQueueElement(CraftRecipe recipe, int amountToCraft = 1)
    {
        this.recipe = recipe;
        this.amountToCraft = amountToCraft;
    }
    
    public bool IsLastCraft(){
        if(amountToCraft > 1){
            return false;
        }
        else{
            return true;
        }
    }
    
    public void Add(){
        amountToCraft++;
    }
    public void Subtract(){
        amountToCraft--;
    }
}
