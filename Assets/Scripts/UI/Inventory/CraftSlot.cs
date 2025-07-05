using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityTxt;
    [SerializeField] private Image rarityBg;
    
    [SerializeField] private RectTransform rectTransform;
    
    public CraftRecipe recipe;
    
    public event Action<CraftRecipe> OnCraftClicked;
    
    private BuildingInventory parentInventory;
    public void Init(BuildingInventory parentInventory)
    {
        this.parentInventory = parentInventory;
    }
    
    public void SetCraftData(Sprite sprite, int quantity, CraftRecipe recipe)
    {
        this.recipe = recipe;
        itemImage.sprite = sprite;
        quantityTxt.text = quantity.ToString();
        //rarityBg.color = Helper.GetRarityColor(recipe.finalItem.rarity);

        quantityTxt.gameObject.SetActive(true);
        this.itemImage.gameObject.SetActive(true);
    }
    
    public CraftQueueElement craftQueueElement;
    public void SetQueueData(Sprite sprite, int amountToCraft, CraftQueueElement queueElement) // НЕ НУЖНО
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
        // int[] amounts = parentInventory.inventory.GetItemAmounts(recipe.requirements.requirements.ToArray());
        // foreach(var req in recipe.requirements.requirements)
        // {
        //     foreach(int n in amounts)
        //     {
        //         if(n < req.quantity){
        //             rarityBg.color = GameAssets.colors.blockedColor;
        //             return;
        //         }
        //         else {
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
    private void ShowTooltip()
    {
        MouseTooltip.ShowTooltip_Static(recipe.finalItem.itemName, 
        recipe.requirements.GetRequirementsCompare(
            parentInventory.inventory.GetItemAmounts(recipe.requirements.requirements.ToArray())));
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
