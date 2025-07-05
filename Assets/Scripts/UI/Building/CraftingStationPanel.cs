using System.Collections.Generic;
using UnityEngine;

public class CraftingStationPanel : BuildingPanel
{
    [SerializeField] private RectTransform craftsContainer;
    [SerializeField] private RectTransform queueContainer;
    public RectTransform outputContainer;
    
    public List<CraftSlot> crafts;
    
    public Bar craftProgressBar;    
    public List<CraftSlot> queueCrafts;
    
    public CraftingStation craftingStation;
    
    public override void Init(Building building, BuildingPanelManager manager)
    {
        base.Init(building, manager);
        
        if(building is CraftingStation cs){
            this.craftingStation = cs;
        }
        
        craftProgressBar.Init();
        
        craftingStation.originInv.inventory.OnInventoryUpdated += OnInventoryUpdated;
        craftingStation.OnQueueChanged += InitQueue;
        
        craftingStation.OnCraftProgressChanged += craftProgressBar.UpdateBar;
        //OnInventoryUpdated();
    }
    protected override void OnDestroy(){
        base.OnDestroy();
        craftingStation.originInv.inventory.OnInventoryUpdated -= OnInventoryUpdated;
        craftingStation.OnCraftProgressChanged -= craftProgressBar.UpdateBar;
        craftingStation.OnQueueChanged -= InitQueue;
        foreach(CraftSlot slot in crafts){
            slot.OnCraftClicked -= craftingStation.QueueCraft;
        }
        foreach(CraftSlot slot in queueCrafts){
            slot.OnCraftClicked -= craftingStation.DequeueCraft;
        }
    }
    
    public void OnInventoryUpdated(Dictionary<int, InventoryItem> invState = null){
        UpdateCraftsAvailability();
    }
    public void UpdateCraftsAvailability()
    {
        if(crafts.Count <= 0) {Debug.LogError("No craft slots initiated. Calling this meathod too early."); return;}
        
        foreach(CraftSlot slot in crafts)
        {
            slot.UpdateAvailability();
        }
    }
    
    public void InitCrafts(CraftRecipe[] recipes)
    {
        foreach (var craft in recipes)
        {
            GameObject go = Instantiate(GameAssets.craftSlot, craftsContainer);
            CraftSlot slot = go.GetComponent<CraftSlot>();
            slot.Init(craftingStation.originInv);
            slot.SetCraftData(craft.finalItem.icon, craft.finalAmount, craft);
            slot.OnCraftClicked += craftingStation.QueueCraft;
            crafts.Add(slot);
        }
        
    }
    
    public void InitQueue(List<CraftQueueElement> queue)
    {
        ClearQueue();
        
        foreach(var craft in queue)
        {
            GameObject go = Instantiate(GameAssets.craftSlot, queueContainer);
            CraftSlot slot = go.GetComponent<CraftSlot>();
            slot.Init(craftingStation.originInv);
            slot.SetQueueData(craft.recipe.finalItem.icon, craft.amountToCraft, craft);
            slot.OnCraftClicked += craftingStation.DequeueCraft;
            queueCrafts.Add(slot);
        }
    }
    public void ClearQueue(){
        for(int i = 0; i < queueCrafts.Count; i++)
        {
            Destroy(queueCrafts[i].gameObject);
        }
        queueCrafts.Clear();
    }
}
