using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outpost : Building
{
    public bool captureInProgress = false;
    
    public OutpostData outpostData;
    public override void Init(BuildingData data)
    {
        base.Init(data);
        
        if(data is OutpostData outpostData){
            this.outpostData = outpostData;
        }
        
        TryStartCapture();
        
        cell.OnCellBuildingsChanged += TryStartCapture;
        //colony = new PlayerColony(this, colonyData, Affiliation.Player);    
    }
    
    public virtual void TryStartCapture()
    {
        if(captureInProgress){
            Debug.LogWarning("Capture already in progress.");
            return;
        }
        
        List<Building> cellBuildings = new List<Building>();
        foreach(Collider c in ColoniesManager.i.gridManager.GetCellBuildings(cellIndex.x, cellIndex.y)){
            cellBuildings.Add(c.GetComponent<Building>());
        }
        
        if(cellBuildings.Find(b => b.affiliation != outpostData.affiliation)){ // если в клетке нашли здание не нашей affiliation
            Debug.LogWarning("Can't start capture. Enemy buildings in cell");
            return;
        }
        
        if(cell.affiliation == Affiliation.None)
        {
            captureRoutine = StartCoroutine(CaptureNeutralCell());
        }
        else if(cell.affiliation != Affiliation.None && cell.affiliation != outpostData.affiliation)
        {
            captureRoutine = StartCoroutine(CaptureEnemyCell());
        }
    }
    
    Coroutine captureRoutine;
    public IEnumerator CaptureNeutralCell()
    {
        cell.StartCapture(affiliation);
        
        float progress = cell.captureProgress;
        while(progress < 1)
        {
            cell.captureProgress = progress;
            
            progress += outpostData.captureSpeed*Time.deltaTime;
            yield return null;
        }
        
        cell.Capture(affiliation);
    }
    public IEnumerator CaptureEnemyCell()
    {
        cell.StartCapture(affiliation);
        
        // uncapture
        float progress = cell.captureProgress;
        while(progress > 0)
        {
            cell.captureProgress = progress;
            
            progress -= outpostData.captureSpeed*Time.deltaTime;
            yield return null;
        }
        
        // capture
        progress = cell.captureProgress;
        while(progress < 1)
        {
            cell.captureProgress = progress;
            
            progress += outpostData.captureSpeed*Time.deltaTime;
            yield return null;
        }
        
        cell.Capture(affiliation);
    }
    
    public override void Death(){
        StopCoroutine(captureRoutine);
        cell.ResetCapture();
        base.Death();
    }
}
