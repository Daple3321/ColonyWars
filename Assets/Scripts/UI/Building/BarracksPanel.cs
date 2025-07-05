using System.Collections.Generic;
using UnityEngine;

public class BarracksPanel : BuildingPanel
{
    [SerializeField] private RectTransform craftsContainer;
    [SerializeField] private RectTransform queueContainer;
    
    public List<UnitCraftSlot> crafts;
    
    public Bar craftProgressBar;    
    public List<UnitCraftSlot> queueCrafts;
    
    public Barracks barracks;
    
    public override void Init(Building building, BuildingPanelManager manager)
    {
        base.Init(building, manager);
        
        if(building is Barracks br){
            this.barracks = br;
        }
        
        craftProgressBar.Init();
        
        barracks.originInv.inventory.OnInventoryUpdated += OnInventoryUpdated;
        barracks.OnQueueChanged += InitQueue;
        
        barracks.OnCraftProgressChanged += craftProgressBar.UpdateBar;
        //OnInventoryUpdated();
    }
    protected override void OnDestroy(){
        base.OnDestroy();
        barracks.originInv.inventory.OnInventoryUpdated -= OnInventoryUpdated;
        barracks.OnCraftProgressChanged -= craftProgressBar.UpdateBar;
        barracks.OnQueueChanged -= InitQueue;
        foreach(UnitCraftSlot slot in crafts){
            slot.OnCraftClicked -= barracks.QueueCraft;
        }
        foreach(UnitCraftSlot slot in queueCrafts){
            slot.OnCraftClicked -= barracks.DequeueCraft;
        }
    }
    
    public void OnInventoryUpdated(Dictionary<int, InventoryItem> invState = null){
        UpdateCraftsAvailability();
    }
    public void UpdateCraftsAvailability()
    {
        if(crafts.Count <= 0) {Debug.LogError("No craft slots initiated. Probably calling this meathod too early."); return;}
        
        foreach(UnitCraftSlot slot in crafts)
        {
            slot.UpdateAvailability();
        }
    }
    
    public void InitCrafts(UnitRecipe[] recipes)
    {
        foreach (var craft in recipes)
        {
            GameObject go = Instantiate(GameAssets.unitCraftSlot, craftsContainer);
            UnitCraftSlot slot = go.GetComponent<UnitCraftSlot>();
            slot.Init(barracks.originInv);
            slot.SetCraftData(craft.unit.icon, 1, craft);
            slot.OnCraftClicked += barracks.QueueCraft;
            crafts.Add(slot);
        }
        
    }
    
    public void InitQueue(List<UnitQueueElement> queue)
    {
        ClearQueue();
        
        foreach(var craft in queue)
        {
            GameObject go = Instantiate(GameAssets.unitCraftSlot, queueContainer);
            UnitCraftSlot slot = go.GetComponent<UnitCraftSlot>();
            slot.Init(barracks.originInv);
            slot.SetQueueData(craft.recipe.unit.icon, craft.amountToCraft, craft);
            slot.OnCraftClicked += barracks.DequeueCraft;
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
