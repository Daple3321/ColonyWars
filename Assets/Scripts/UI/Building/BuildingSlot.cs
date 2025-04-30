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

    public RectTransform rectTransform;
    
    public void Init(BuildingData buildingData)
    {
        this.buildingData = buildingData;
        buildingIcon.sprite = buildingData.buildingIcon;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotClicked?.Invoke(this);
    }
    
    Tween scaleTween;
    public void OnPointerEnter(PointerEventData eventData)
    {
        scaleTween.Stop();
        scaleTween = Tween.Scale(rectTransform, 1.15f, 0.15f, Ease.OutCubic);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        scaleTween = Tween.Scale(rectTransform, 1, 0.15f, Ease.InCubic);
    }
}
