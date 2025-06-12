using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        
        cell.OnCellBuildingsChanged += ()=>{ // bullshit delay hack
            if(captureDelay != null)
                StopCoroutine(captureDelay);
            captureDelay = StartCoroutine(CaptureDelay());
        };
        //colony = new PlayerColony(this, colonyData, Affiliation.Player);    
    }
    
    Coroutine captureDelay;
    private IEnumerator CaptureDelay()
    {
        yield return new WaitForSeconds(0.3f);
        TryStartCapture();
    }
    
    private bool canStartCapture = false;
    public virtual void TryStartCapture()
    {
        if(captureInProgress){
            Debug.LogWarning("Capture already in progress.");
            return;
        }
        
        List<Building> cellBuildings = new List<Building>();
        foreach(Collider c in ColoniesManager.i.gridManager.GetCellBuildings(cellIndex.x, cellIndex.z)){
            cellBuildings.Add(c.GetComponent<Building>());
        }
        
        if(cellBuildings.Find(b => b.affiliation != outpostData.affiliation)){ // если в клетке нашли здание не нашей affiliation
            Debug.LogWarning("Can't start capture. Enemy buildings in cell");
            canStartCapture = false;
            return;
        }
        
        
        if(cell.affiliation == Affiliation.None)
        {
            canStartCapture = true;
            captureRoutine = StartCoroutine(CaptureNeutralCell());
        }
        else if(cell.affiliation != Affiliation.None && cell.affiliation != outpostData.affiliation)
        {
            canStartCapture = true;
            captureRoutine = StartCoroutine(CaptureEnemyCell());
        }
    }
    
    Coroutine captureRoutine;
    public IEnumerator CaptureNeutralCell()
    {
        cell.StartCapture(affiliation);
        captureInProgress = true;
        
        float progress = cell.captureProgress;
        while(progress < 1)
        {
            cell.captureProgress = progress;
            
            progress += outpostData.captureSpeed*Time.deltaTime;
            yield return null;
        }
        
        cell.Capture(affiliation);
        captureInProgress = false;
    }
    public IEnumerator CaptureEnemyCell()
    {
        cell.StartCapture(affiliation);
        captureInProgress = true;
        
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
        captureInProgress = false;
    }
    
    StringBuilder sb = new StringBuilder();
    protected override void OnMouseOver()
    {
        sb.Clear();
        if(canStartCapture){
            sb.AppendLine($"<color=green>Can capture</color>");
        }
        else{
            sb.AppendLine($"<color=red>Can't start capture</color>");
        }
        sb.AppendLine($"Capture progress: {cell.captureProgress:F1}");
        sb.AppendLine($"Yield amount: {cell.yieldAmount:F1}");
        
        WorldUI.i.buildingHover.UpdateInfo(sb.ToString());
    }
    
    public override void Death(){
        if(captureRoutine != null){
            StopCoroutine(captureRoutine);
        }
        
        cell.ResetCapture();
        base.Death();
    }
}
