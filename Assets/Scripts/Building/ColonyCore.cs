using UnityEngine;

public class ColonyCore : Building
{
    public override BuildingPanel CreatePanel(RectTransform parentContainer)
    {
        GameObject go = Instantiate(GameAssets.colonyCenterPanel, parentContainer);
        ColonyCenterPanel panel = go.GetComponent<ColonyCenterPanel>();
        panel.Init(this);
        return panel;
    }
}
