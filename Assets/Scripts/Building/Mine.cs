using UnityEngine;

public class Mine : ResourceGenerator
{
    public override void OnClick(Player caller)
    {
        if(built && affiliation == Affiliation.Player){
            if(Vector3.Distance(transform.position, caller.transform.position) <= interactionRange){
                //Debug.Log($"Clicked on {buildingData.buildingName}");
                caller.buildingPanelManager.CreatePanel(this);
                //UpdateUI(buildingInventory.inventory.GetCurrentInventoryState());
            }
        }
    }
    
    public override BuildingPanel CreatePanel(RectTransform parentContainer)
    {
        GameObject go = Instantiate(GameAssets.minePanel, parentContainer);
        MinePanel panel = go.GetComponent<MinePanel>();
        //panel.Init(this);
        return panel;
    }
}
