using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PrimeTween;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityTxt;
    [SerializeField] private Image borderImage;
    public InventoryItem item;

    public event Action<InventorySlot> OnItemClicked, OnRightClick, OnItemDropped,
        OnItemBeginDrag, OnItemEndDrag;


    public bool empty = true;

    public InventoryUI ParentUI { get; private set; }

    private RectTransform rectTransform;
    public void Init(InventoryUI parentUI)
    {
        ParentUI = parentUI;
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ResetData();
        Deselect();
    }

    public void ResetData()
    {
        this.itemImage.gameObject.SetActive(false);
        quantityTxt.gameObject.SetActive(false);
        item = InventoryItem.GetEmptyItem();
        empty = true;
    }

    public void Select()
    {
        borderImage.enabled = true;
    }
    public void Deselect()
    {
        borderImage.enabled = false;
    }

    public void SetData(Sprite sprite, int quantity, InventoryItem item)
    {
        this.item = item;
        itemImage.sprite = sprite;
        quantityTxt.text = quantity.ToString();

        if (item.item.itemData.IsStackable)
        {
            quantityTxt.gameObject.SetActive(true);
        }
        else
        {
            quantityTxt.gameObject.SetActive(false);
        }
        this.itemImage.gameObject.SetActive(true);
        empty = false;
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick?.Invoke(this);
        }
        else
        {
            OnItemClicked?.Invoke(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (empty)
            return;
        //Debug.Log("Slot is not empty. Beggining drag.", this);
        OnItemBeginDrag?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnItemEndDrag?.Invoke(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        OnItemDropped?.Invoke(this); // sending slot that we dropped into
        //Debug.Log("Dropped to slot", this);
    }

    public void OnDrag(PointerEventData eventData)
    {

    }
    
    
    private void ShowTooltip()
    {
        MouseTooltip.ShowTooltip_Static(item.item.itemName, item.item.GetDescription());
    }
    Tween scaleTween;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!empty)
            ShowTooltip();

        //rectTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.18f).SetEase(Ease.OutCubic)
        //.SetUpdate(true).SetRecyclable();

        scaleTween.Stop();
        scaleTween = Tween.Scale(rectTransform, 1.15f, 0.15f, Ease.OutCubic);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        scaleTween = Tween.Scale(rectTransform, 1, 0.15f, Ease.InCubic);
        MouseTooltip.HideTooltip_Static();
        
        //rectTransform.DOScale(new Vector3(1, 1, 1), 0.18f).SetEase(Ease.OutCubic)
        //.SetRecyclable();
    }
}
