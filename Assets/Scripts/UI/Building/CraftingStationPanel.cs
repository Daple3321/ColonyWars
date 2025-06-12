using System.Collections.Generic;
using UnityEngine;

public class CraftingStationPanel : BuildingPanel
{
    [SerializeField] private RectTransform craftsContainer;
    [SerializeField] private RectTransform queueContainer;
    
    public List<CraftSlot> crafts;
    
    public List<CraftSlot> queueCrafts;
    
    public CraftingStation craftingStation;
    
    public override void Init(Building building, BuildingPanelManager manager)
    {
        base.Init(building, manager);
        
        if(building is CraftingStation cs){
            this.craftingStation = cs;
        }
        
        craftingStation.originInv.inventory.OnInventoryUpdated += OnInventoryUpdated;
        
        //OnInventoryUpdated();
    }
    protected override void OnDestroy(){
        base.OnDestroy();
        craftingStation.originInv.inventory.OnInventoryUpdated -= OnInventoryUpdated;
    }
    
    public void OnInventoryUpdated(Dictionary<int, InventoryItem> invState = null){
        foreach(CraftSlot slot in crafts)
        {
            slot.UpdateAvailability();
        }
    }
    
    public void InitCrafts(CraftRecipe[] recipes)
    {
        foreach (CraftRecipe craft in recipes)
        {
            GameObject go = Instantiate(GameAssets.craftSlot, craftsContainer);
            CraftSlot slot = go.GetComponent<CraftSlot>();
            slot.Init(craftingStation.originInv);
            slot.SetData(craft.finalItem.icon, craft.finalAmount, craft);
            crafts.Add(slot);
        }
    }
}
