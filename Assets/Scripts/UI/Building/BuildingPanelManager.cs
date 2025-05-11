using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPanelManager : MonoBehaviour
{
    public BuildingPanel currentPanel;
    public RectTransform panelRectTransform;
    
    public Canvas buildingCanvas;
    public RectTransform panelParent;
    
    private bool canHide;
    
    public void Init()
    {
        ClearCurrentPanel();
        buildingCanvas = GetComponent<Canvas>();
        
        //Debug.Log($"Scale factor: {buildingCanvas.scaleFactor}");
    }
    
    // Сделать ShowPanel(Building building) с проверками на уже существую панельку
    // HidePanel() при нажатии вне панели
    
    public void CreatePanel(Building building)
    {
        ClearCurrentPanel();
        
        //Debug.Log($"Showing panel for {building.buildingData.buildingName}");
        currentPanel = building.CreatePanel(panelParent);
        currentPanel.Init(building, this);
        panelRectTransform = currentPanel.GetComponent<RectTransform>();
        
        currentPanel.building.OnBuildingDestroyed += x => {
            ClearCurrentPanel(); 
            StopCoroutine(delayRoutine); 
            canHide = false;
        };
        
        currentPanel.building.PrepareUI();
        
        if(delayRoutine != null){
            StopCoroutine(delayRoutine);
        }
        delayRoutine = StartCoroutine(HideDelay());
        
        Canvas.ForceUpdateCanvases();
    }
    
    Coroutine delayRoutine;
    IEnumerator HideDelay()
    {
        yield return new WaitForSeconds(0.1f);
        canHide = true;
    }
    
    void Update()
    {
        if(currentPanel != null && canHide){
            HideIfClickedOutside();
            //HideIfOutsideRange();
        }
    }
    
    private void HideIfOutsideRange()
    {
        if(Vector3.Distance(GameController.p.transform.position, currentPanel.building.transform.position) > currentPanel.building.interactionRange){
            ClearCurrentPanel();
            canHide = false;
        }
    }
    
    private void HideIfClickedOutside() 
    {
        // if (Input.GetMouseButton(0) && currentPanel.gameObject.activeSelf)
        // {
        //     if(!RectTransformUtility.RectangleContainsScreenPoint(panelRectTransform,
        //         Input.mousePosition,
        //         null)) 
        //     {
        //         HidePanel();
        //         canHide = false;
        //     }
            
        // }
        
        if(Input.GetMouseButtonDown(0))
        {
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                //HidePanel();
                ClearCurrentPanel();
                canHide = false;
            }
        }
        
    }
    
    public void HidePanel()
    {
        currentPanel.Hide();
    }
    
    public void ClearCurrentPanel()
    {
        if(currentPanel != null){
            currentPanel.building.ClearUI();
            
            currentPanel.Hide(()=>{
                Destroy(currentPanel.gameObject);
            });
            
            currentPanel = null;
            canHide = false;
        }
    }
    
}
