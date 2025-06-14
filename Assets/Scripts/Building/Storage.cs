using UnityEngine;

public class Storage : Building
{
    public BuildingInventory inv;
    
    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        inv = new BuildingInventory(this, 6);
    }
    public override void PrepareUI()
    {
        inv.PrepareUI();
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
