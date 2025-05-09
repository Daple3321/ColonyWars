using UnityEngine;

public class ColonyCore : Building
{
    public ColonyCenterData colonyData;
    
    public Colony colony;
    
    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        if(data is ColonyCenterData colonyCenterData){
            this.colonyData = colonyCenterData;
        }
        
        colony = new PlayerColony(this, colonyData, Affiliation.Player);    
    }
    
    public override void Death(){
        EventBus.i.OnColonyDestroyed?.Invoke(colony);
        base.Death();
    }
    
    public override BuildingPanel CreatePanel(RectTransform parentContainer)
    {
        GameObject go = Instantiate(GameAssets.colonyCenterPanel, parentContainer);
        ColonyCenterPanel panel = go.GetComponent<ColonyCenterPanel>();
        //panel.Init(this);
        return panel;
    }
}
