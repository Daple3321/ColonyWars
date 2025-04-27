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

    public event Action<InventorySlot> OnItemClicked, OnRightClick, OnItemEndDrag;
    
    public event Action<InventorySlot, int> OnItemVoidDrop;
    public event Action<InventorySlot, int> OnItemDropped;
    public event Action<InventorySlot, bool> OnItemBeginDrag;

    public bool empty = true;
    
    private bool canDrop;
    private bool rightClickDrag;

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
        if (empty){
            //Debug.Log($"[OnBeginDrag] slot empty. Returning.", this);
            return;
        }
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Debug.Log("BeginDrag with right click");
            rightClickDrag = true;
            OnItemBeginDrag?.Invoke(this, true);
        }
        else{
            rightClickDrag = false;
            OnItemBeginDrag?.Invoke(this, false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool droppedOnOriginSlot = false;
        // Проверяем, находится ли указатель мыши над объектом UI
        if (eventData.pointerEnter != null)
        {
            // Пытаемся получить компонент InventorySlot из объекта под указателем
            //InventorySlot slotUnderPointer = eventData.pointerEnter.GetComponentInParent<InventorySlot>();
            InventorySlot slotUnderPointer = eventData.pointerDrag.GetComponent<InventorySlot>();
            // Сравниваем найденный слот с тем, откуда начали перетаскивание (хранится в DragDropManager)
            if (slotUnderPointer != null && slotUnderPointer == DragDropManager.currentlyDraggedSlot)
            {
                droppedOnOriginSlot = true;
                // Можно добавить Debug.Log("Dropped back onto the origin slot.");
            }
        }
        
        // --- Логика выброса в пустоту ---
        // Выбрасываем только если:
        // 1. Не было успешного OnDrop на каком-либо слоте (canDrop == false)
        // 2. И курсор НЕ был отпущен над исходным слотом (droppedOnOriginSlot == false)
        if(!canDrop && !droppedOnOriginSlot)
        {
            if(rightClickDrag)// if dropped in void and halfStack drop
            {
                if(item.IsStackable){
                    OnItemVoidDrop?.Invoke(this, item.HalfQuantity());
                }
                else{
                    OnItemVoidDrop?.Invoke(this, item.quantity);
                }
                //Debug.Log($"[OnEndDrag] Void halfStack drop", this);
            }
            else if(!canDrop && !rightClickDrag){
                OnItemVoidDrop?.Invoke(this, -1);
                //Debug.Log($"[OnEndDrag] Void drop", this);
            }
        }
        //Debug.Log($"[OnEndDrag]", this);
        OnItemEndDrag?.Invoke(this);
        canDrop = false;
        rightClickDrag = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot dropSlot = eventData.pointerDrag.GetComponent<InventorySlot>();
        if(dropSlot != null){ // check for drop in void
            dropSlot.canDrop = true;
            if(rightClickDrag){
                OnItemDropped?.Invoke(this, item.HalfQuantity()); // sending slot that we dropped into
            }
            else{
                OnItemDropped?.Invoke(this, item.quantity);
            }
            
            
            //Debug.Log($"[OnDrop] dropped in {dropSlot}", dropSlot);
        }
        
        
        //Debug.Log("Dropped to slot", this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log($"DragAmount: {DragDropManager.dragQuantity}");
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
