using UnityEngine;

public class Storage : Building
{
    public BuildingInventory inv;
    
    public int inventorySize = 12;
    
    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        inv = new BuildingInventory(this, inventorySize);
    }
    public override void PrepareUI()
    {
        inv.PrepareUI(true, GameController.p.playerInventory.inventoryUI, null);
        inv.UpdateUI(inv.inventory.GetCurrentInventoryState());
        EventBus.i.OnInventoryPanelOpened?.Invoke();
    }
    public override void ClearUI()
    {
        inv.ClearUI();
        EventBus.i.OnInventoryPanelClosed?.Invoke();
    }
    public override void OnDeath()
    {
        base.OnDeath();
        inv.DropAllItems();
    }
    
    
}
