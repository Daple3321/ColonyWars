using System.Collections;
using System.Text;
using UnityEngine;

public class ResourceRefiner : Building
{
    public BuildingInventory originInv;
    public BuildingInventory refinedInv;
    
    private ResourceRefinerData refinerData;
    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        originInv = new BuildingInventory(this, 2);
        refinedInv = new BuildingInventory(this, 2);
        
        if(data is ResourceRefinerData resourceRefinerData){
            this.refinerData = resourceRefinerData;
        }
        
        refineDelay = refinerData.refineSpeed;
    }
    public override void PrepareUI()
    {
        originInv.PrepareUI(true, GameController.p.playerInventory.inventoryUI);
        originInv.UpdateUI(originInv.inventory.GetCurrentInventoryState());
        
        refinedInv.PrepareUI(false, GameController.p.playerInventory.inventoryUI);
        refinedInv.UpdateUI(refinedInv.inventory.GetCurrentInventoryState());
    }
    public override void ClearUI()
    {
        originInv.ClearUI();
        refinedInv.ClearUI();
    }
    public override void OnDeath()
    {
        base.OnDeath();
        
        originInv.DropAllItems();
        refinedInv.DropAllItems();
    }


    void Update()
    {
        HandleRefining();
    }

    public float refineDelay;
    public bool refineInProgress;
    protected virtual void HandleRefining()
    {
        if(refineDelay > 0){
            refineDelay -= Time.deltaTime;
        }
        else if(refineDelay <= 0f){
            Refine();
            
            if(GetCurrentRecipe() != null)
            {
                refineDelay = GetCurrentRecipe().craftTime;
            }
        }
    }
    protected virtual void Refine()
    {
        if(HasRecipe() && CanRefine())
        {
            CraftRecipe recipe = GetCurrentRecipe();
            if(recipe != null)
            {
                refinedInv.inventory.AddItem(new Item(recipe.finalItem), recipe.finalAmount);
                originInv.inventory.ConsumeItemRequirements(recipe.requirements);
                // foreach(var req in recipe.itemRequirements.requirements)
                // {
                //     originInv.inventory.DeleteAmount(originInv.inventory.HasItem(req.item), req.quantity);
                // }
            }
        }
    }
    protected bool CanRefine()
    {
        // has empty slot or can stack to existing item
        // BUG: Когда есть нужный но полностью забитый стак, всё равно рефайнится
        if(refinedInv.inventory.EmptySlots() >= 1
        || refinedInv.inventory.HasItemWithSpaceLeft(GetCurrentRecipe().finalItem, GetCurrentRecipe().finalAmount) != -1) // сломано
        {
            return true;
        }
        else{
            return false;
        }
    }
    protected bool HasRecipe()
    {
        bool hasAtleastOneRecipe = false;
        foreach(var recipe in refinerData.recipes)
        {
            bool hasRequirements = true;
            foreach(var req in recipe.requirements.requirements)
            {
                if(originInv.inventory.ItemAmount(req.item) < req.quantity){
                    hasRequirements = false;
                }
            }
            
            if(hasRequirements){
                hasAtleastOneRecipe = true;
            }
        }
        //return buildingInventory.inventory.ItemAmount(refinerData.originItem) >= refinerData.originItemAmount 
        //    && !refineInProgress;
        return hasAtleastOneRecipe;
    }
    protected CraftRecipe GetCurrentRecipe()
    {
        foreach(CraftRecipe recipe in refinerData.recipes)
        {
            bool hasRequirements = true;
            foreach(var req in recipe.requirements.requirements)
            {
                if(originInv.inventory.ItemAmount(req.item) < req.quantity){
                    hasRequirements = false;
                }
            }
            
            if(hasRequirements){
                //Debug.Log($"Current recipe is {recipe.finalItem}");
                return recipe;
            }
        }
        
        //Debug.Log($"Current recipe is NULL");
        return null;
    }
    

    StringBuilder sb = new StringBuilder();
    protected override void OnMouseOver()
    {
        sb.Clear();
        sb.AppendLine($"Smelting progress: {refineDelay:F1}");
        
        WorldUI.i.buildingHover.UpdateInfo(sb.ToString());
    }
}
