using UnityEngine;

public class Mine : ResourceGenerator
{
    public override void OnClick(Player caller)
    {
        if(built){
            if(Vector3.Distance(transform.position, caller.transform.position) <= interactionRange){
                //Debug.Log($"Clicked on {buildingData.buildingName}");
                caller.buildingPanelManager.CreatePanel(this);
                UpdateUI(inventory.GetCurrentInventoryState());
            }
        }
    }
    
    public override BuildingPanel CreatePanel(RectTransform parentContainer)
    {
        GameObject go = Instantiate(GameAssets.minePanel, parentContainer);
        MinePanel panel = go.GetComponent<MinePanel>();
        panel.Init(this);
        return panel;
    }
}
