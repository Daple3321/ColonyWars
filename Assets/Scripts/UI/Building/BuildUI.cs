using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class BuildUI : MonoBehaviour
{
    public List<BuildingSlot> buildingSlots;
    
    public GameObject buildPanel;
    public RectTransform slotGrid;
    
    [SerializeField] private Vector3 startPos;
    private RectTransform rectTransform;
    
    public void Init(PlayerBuilding playerBuilding)
    {
        playerBuilding.availableBuildingsChanged += UpdateUI;
        buildingSlots = new List<BuildingSlot>();
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
        
        Hide();
    }
    
    public event Action<BuildingData> OnSlotClicked;
    public void HandleSlotClick(BuildingSlot slot)
    {
        OnSlotClicked?.Invoke(slot.buildingData);
    }
    
    public void UpdateUI(List<BuildingData> buildings)
    {
        ClearUI();
        
        foreach(BuildingData building in buildings)
        {
            BuildingSlot slot = Instantiate(GameAssets.buildingSlot, Vector3.zero, Quaternion.identity).GetComponent<BuildingSlot>();
            slot.transform.SetParent(slotGrid, false); // можно ещё scale умножить на canvas.scaleFactor
            slot.Init(building);
            slot.OnSlotClicked += HandleSlotClick;
            
            buildingSlots.Add(slot);
        }
    }
    
    public void ClearUI()
    {
        foreach (BuildingSlot slot in buildingSlots)
        {
            Destroy(slot.gameObject);
        }
        
        buildingSlots.Clear();
    }
    
    public void Show(){
        gameObject.SetActive(true);
        Tween.UIAnchoredPosition(rectTransform, startPos, 0.2f, Ease.OutCubic);
        
    }
    
    public void Hide(){
        Tween.UIAnchoredPositionY(rectTransform, -500, 0.25f, Ease.OutQuart)
            .OnComplete(()=>gameObject.SetActive(false));
    }
}
