using System.Collections;
using UnityEngine;

[System.Serializable]
public class PlayerColony : Colony
{
    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        // colonyZone = ZoneFactory.CreateTriggerZone(transform.position, Color.cyan, ZoneShape.Cylinder, colonyRadius, 10f);
        // colonyZone.transform.position = transform.position;
        // colonyZone.OnZoneEnter += x =>  {Debug.Log($"{x.gameObject.name} entered colony zone!");};
        //colonyZone.transform.SetParent(core.transform); // НУ И КАК ЭТО СДЕЛАТЬ ТО???
        EventBus.i.OnPlayerBuild += OnPlayerBuild;
    }
    public override IEnumerator Build()
    {
        float timeLeft = 0;
        while (timeLeft < buildTime)
        {
            buildProgress = timeLeft/buildTime;
            //health = Mathf.Lerp(health, maxHealth, buildProgress);
            
            ChangeColor(Color.Lerp(Color.black, Color.white, buildProgress));
            timeLeft += Time.deltaTime;
            yield return null;
        }
        
        built = true;
        buildProgress = 1f;
        
        if(cell.affiliation != Affiliation.Player && cell.affiliation != Affiliation.Enemy){
            ColoniesManager.i.gridManager.CaptureCell(cellIndex.x, cellIndex.z, Affiliation.Player);
        }
        ChangeColor(Color.white);
    }
    
    public override BuildingPanel CreatePanel(RectTransform parentContainer)
    {
        GameObject go = Instantiate(GameAssets.colonyCenterPanel, parentContainer);
        ColonyCenterPanel panel = go.GetComponent<ColonyCenterPanel>();
        //panel.Init(this);
        return panel;
    }
    
    protected override void OnCoreDestroyed(Building core)
    {
        EventBus.i.OnPlayerBuild -= OnPlayerBuild;
        core.OnBuildingDestroyed -= OnCoreDestroyed;
        
       //GameObject.Destroy(colonyZone.gameObject);
        for(int i = 0; i < buildings.Count; i++)
        {
            buildings[i].Death();
        }
    }
    
    public virtual void OnPlayerBuild(Building building)
    {
        if(Vector3.Distance(building.transform.position, transform.position) < colonyRadius)
        {
            AddBuilding(building);
        }
    }
}
