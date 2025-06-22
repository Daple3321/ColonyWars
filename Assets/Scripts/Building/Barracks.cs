using System;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Manufacturer
{
    public List<UnitRecipe> recipes;
    
    public bool craftInProgress;
    public float craftProgress;
    public UnitQueueElement currentCraft;
    public List<UnitQueueElement> queue;
    public int queueCapacity = 10;
    
    public BarracksData barracksData;
    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        queue = new List<UnitQueueElement>();
        
        if(data is BarracksData bd){
            barracksData = bd;
            recipes = bd.recipes;
        }
    }
    
    public override void PrepareUI()
    {
        originInv.PrepareUI(true, GameController.p.playerInventory.inventoryUI, null, "Resources");
        originInv.UpdateUI(originInv.inventory.GetCurrentInventoryState());
        
        if(GameController.i.buildingPanelManager.currentPanel is BarracksPanel csp)
        {
            csp.InitCrafts(recipes.ToArray());
            csp.InitQueue(queue);
        }
    }
    public override void ClearUI()
    {
        originInv.ClearUI();
        
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
    }
    
    public override BuildingPanel CreatePanel(RectTransform parentContainer){
        GameObject go = Instantiate(GameAssets.barracksPanel, parentContainer);
        BarracksPanel panel = go.GetComponent<BarracksPanel>();
        //panel.Init(this);
        return panel;
    }

    void Update(){
        HandleCraftQueue();
    }

    public Action<float, float> OnCraftProgressChanged;
    public Action<List<UnitQueueElement>> OnQueueChanged;
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
        
        if(craftInProgress){
            HandleCraftProgress();
        }
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
    
    public void QueueCraft(UnitRecipe recipe)
    {
        if(queue.Count >= queueCapacity){return;}
        
        UnitQueueElement foundElement =  queue.Find(x => x.recipe == recipe);
        if(foundElement != null){
            //Debug.Log("Stacking craft!");
            foundElement.Add();
        }
        else{
            queue.Add(new UnitQueueElement(recipe));
        }
        
        OnQueueChanged?.Invoke(queue);
    }
    public void DequeueCraft(UnitRecipe recipe)
    {
        UnitQueueElement foundElement = queue.Find(x => x.recipe == recipe);
        if(foundElement != null){
            queue.Remove(foundElement);
            OnQueueChanged?.Invoke(queue);
        }
        else{
            Debug.LogError("Can't dequeue. Element with that craft not found!");
        }
    }
    
    // public bool CanQueue(UnitRecipe recipe)
    // {
    //     if(outputInv.inventory.EmptySlots() < 1
    //     && outputInv.inventory.HasItemWithSpaceLeft(recipe.unit, recipe.finalAmount) == -1){
    //         return false;
    //     }
        
    //     if(outputInv.inventory.EmptySlots() >= 1 ||
    //         outputInv.inventory.HasItemWithSpaceLeft(recipe.finalItem, recipe.finalAmount) != -1) 
    //     {
    //         return true;
    //     }
        
    //     return true;
    // }
    
    public bool CanCraft(UnitRecipe recipe) // GC alloc every frame bruh this fucking LINQ
    {
        foreach (ItemRequirement req in recipe.requirements.requirements)
        {
            if (originInv.inventory.ItemAmount(req.item) < req.quantity)
                return false;
        }
        // if(outputInv.inventory.EmptySlots() < 1
        // && outputInv.inventory.HasItemWithSpaceLeft(recipe.finalItem, recipe.finalAmount) == -1){
        //     return false;
        // }
        
        // if(outputInv.inventory.EmptySlots() >= 1 ||
        //     outputInv.inventory.HasItemWithSpaceLeft(recipe.finalItem, recipe.finalAmount) != -1) 
        // {
        //     return true;
        // }
        
        return true;
    }

    public void Craft(UnitRecipe recipe)
    {
        if (!CanCraft(recipe)) return;

        originInv.inventory.ConsumeItemRequirements(recipe.requirements);
        
        Unit spawnedUnit = SpawnUnit(recipe.unit);
        spawnedUnit.Init();
        
        //outputInv.inventory.AddItem(recipe.unit.CreateItemInstance(), recipe.finalAmount);

        return;
    }
    
    protected Unit SpawnUnit(UnitData unit)
    {
        Vector2 spawnPos = new Vector2(transform.position.x, transform.position.z);
        
        Vector3 pointInCircle = GameController.RandomPointInCircleTerrain(spawnPos, 2, 4);
        GameObject go = Instantiate(unit.prefab, pointInCircle, Quaternion.identity);
        Unit u = go.GetComponent<Unit>();

        return u;
    }
}

[System.Serializable]
public class UnitQueueElement
{
    public UnitRecipe recipe;
    public int amountToCraft;
    
    public UnitQueueElement(UnitRecipe recipe, int amountToCraft = 1)
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