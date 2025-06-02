using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public BuildingData buildingData;
    
    public Action<BuildingSlot> OnSlotClicked;
    
    public Image buildingIcon;
    public Image statusImg;

    public RectTransform rectTransform;
    
    private PlayerInventory playerInventory;
    private UIState state;
    public void Init(BuildingData buildingData)
    {
        this.buildingData = buildingData;
        buildingIcon.sprite = buildingData.buildingIcon;
        
        playerInventory = GameController.p.playerInventory;
        playerInventory.OnInventoriesUpdated += UpdateState;
        
        UpdateState();
    }
    
    public void UpdateState()
    {
        if(CheckBuildPrice()){
            statusImg.color = GameAssets.colors.availableColor;
            state = UIState.Enabled;
        }
        else{
            statusImg.color = GameAssets.colors.blockedColor;
            state = UIState.Blocked;
        }
    }
    public bool CheckBuildPrice()
    {
        return playerInventory.CheckItemRequirements(buildingData.craftPrice);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(state == UIState.Enabled)
        {
            OnSlotClicked?.Invoke(this);
        }
    }
    
    private void ShowTooltip()
    {
        string finalString = "";
        
        finalString += buildingData.description + "\n";
        
        finalString += buildingData.craftPrice.GetRequirementsCompare(
            playerInventory.GetItemAmounts(buildingData.craftPrice.requirements.ToArray()));
            
        
        MouseTooltip.ShowTooltip_Static(buildingData.buildingName, finalString);
    }
    
    Tween scaleTween;
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
        
        if(state != UIState.Blocked){
            scaleTween.Stop();
            scaleTween = Tween.Scale(rectTransform, 1.15f, 0.15f, Ease.OutCubic);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseTooltip.HideTooltip_Static();
        
        if(state != UIState.Blocked){
            scaleTween = Tween.Scale(rectTransform, 1, 0.15f, Ease.InCubic);
        }
    }
}

public enum UIState : byte
{
    Enabled,
    Blocked,
}