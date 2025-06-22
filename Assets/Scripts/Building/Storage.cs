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
    }
    public override void ClearUI()
    {
        inv.ClearUI();
    }
    public override void OnDeath()
    {
        base.OnDeath();
        inv.DropAllItems();
    }
    
    
}
