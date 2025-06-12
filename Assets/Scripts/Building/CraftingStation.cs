using System.Collections.Generic;
using UnityEngine;

public class CraftingStation : Manufacturer
{
    public List<CraftRecipe> recipes;
    
    public BuildingInventory outputInv;
    public CraftingStationData stationData;
    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        outputInv = new BuildingInventory(this, 1);
        
        if(data is CraftingStationData csd){
            stationData = csd;
            recipes = csd.recipes;
        }
    }
    
    public override void PrepareUI()
    {
        originInv.PrepareUI();
        originInv.UpdateUI(originInv.inventory.GetCurrentInventoryState());
        outputInv.PrepareUI();
        outputInv.UpdateUI(outputInv.inventory.GetCurrentInventoryState());
        
        if(GameController.i.buildingPanelManager.currentPanel is CraftingStationPanel csp)
        {
            csp.InitCrafts(recipes.ToArray());
            foreach(CraftSlot slot in csp.crafts){
                slot.OnCraftClicked += Craft;
            }
        }
    }
    public override void ClearUI()
    {
        originInv.ClearUI();
        outputInv.ClearUI();
        
        if(GameController.i.buildingPanelManager.currentPanel is CraftingStationPanel csp)
        {
            foreach(CraftSlot slot in csp.crafts){
                slot.OnCraftClicked -= Craft;
            }
        }
    }
    public override void OnDeath()
    {
        base.OnDeath();
        originInv.DropAllItems();
        outputInv.DropAllItems();
    }
    
    public override BuildingPanel CreatePanel(RectTransform parentContainer)
    {
        GameObject go = Instantiate(GameAssets.craftingStationPanel, parentContainer);
        CraftingStationPanel panel = go.GetComponent<CraftingStationPanel>();
        //panel.Init(this);
        return panel;
    }
    
    public bool CanCraft(CraftRecipe recipe)
    {
        foreach (var req in recipe.requirements.requirements)
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
