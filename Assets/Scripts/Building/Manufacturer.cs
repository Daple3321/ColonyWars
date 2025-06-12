using UnityEngine;

public class Manufacturer : Building
{
    public BuildingInventory originInv;
    
    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        originInv = new BuildingInventory(this, 6);
    }
    public override void PrepareUI()
    {
        originInv.PrepareUI();
        originInv.UpdateUI(originInv.inventory.GetCurrentInventoryState());
    }
    public override void ClearUI()
    {
        originInv.ClearUI();
    }
    public override void OnDeath()
    {
        base.OnDeath();
        originInv.DropAllItems();
    }
    
    
}
