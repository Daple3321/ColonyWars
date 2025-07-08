using System;
using System.Text;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitCraftSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityTxt;
    [SerializeField] private Image rarityBg;
    
    [SerializeField] private RectTransform rectTransform;
    
    public UnitRecipe recipe;
    
    public event Action<UnitRecipe> OnCraftClicked;
    
    private BuildingInventory parentInventory;
    public void Init(BuildingInventory parentInventory)
    {
        this.parentInventory = parentInventory;
    }
    
    public void SetCraftData(Sprite sprite, int quantity, UnitRecipe recipe)
    {
        this.recipe = recipe;
        itemImage.sprite = sprite;
        quantityTxt.text = quantity.ToString();
        //rarityBg.color = Helper.GetRarityColor(recipe.finalItem.rarity);

        quantityTxt.gameObject.SetActive(true);
        this.itemImage.gameObject.SetActive(true);
    }
    
    public UnitQueueElement craftQueueElement;
    public void SetQueueData(Sprite sprite, int amountToCraft, UnitQueueElement queueElement) // НЕ НУЖНО
    {
        this.craftQueueElement = queueElement;
        this.recipe = queueElement.recipe;
        itemImage.sprite = sprite;
        quantityTxt.text = amountToCraft.ToString();

        quantityTxt.gameObject.SetActive(true);
        this.itemImage.gameObject.SetActive(true);
    }
    
    public void UpdateAvailability()
    {
        // foreach(var req in recipe.requirements.requirements)
        // {
        //     int[] amounts = parentInventory.inventory.GetItemAmounts(recipe.requirements.requirements.ToArray());
        //     foreach(int n in amounts)
        //     {
        //         if(n < req.quantity){
        //             rarityBg.color = GameAssets.colors.blockedColor;
        //             return;
        //         }
        //         else{
        //             rarityBg.color = GameAssets.colors.availableColor;
        //         }
        //     }
        // }
        
        if(!parentInventory.inventory.CheckItemRequirements(recipe.requirements)){
            rarityBg.color = GameAssets.colors.blockedColor;
        }
        else{
            rarityBg.color = GameAssets.colors.availableColor;
        }
    }
    
    public void OnPointerClick(PointerEventData pointerData)
    {
        OnCraftClicked?.Invoke(recipe);
    }
    StringBuilder sb = new StringBuilder();
    private void ShowTooltip()
    {
        sb.Clear();
        sb.AppendLine(recipe.unit.description);
        sb.Append(recipe.requirements.GetRequirementsCompare(
            parentInventory.inventory.GetItemAmounts(recipe.requirements.requirements.ToArray())));
        
        MouseTooltip.ShowTooltip_Static(recipe.unit.unitName, sb.ToString());
    }
    Tween scaleTween;
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();

        scaleTween.Stop();
        scaleTween = Tween.Scale(rectTransform, 1.15f, 0.15f, Ease.OutCubic);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        scaleTween = Tween.Scale(rectTransform, 1, 0.15f, Ease.InCubic);
        MouseTooltip.HideTooltip_Static();
    }
}
